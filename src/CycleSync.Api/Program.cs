using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddMemoryCache();
builder.Services.AddResponseCompression(options => options.EnableForHttps = true);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

var authSettings = builder.Configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>() ?? new AuthSettings();
if (string.IsNullOrWhiteSpace(authSettings.SigningKey))
{
    authSettings.SigningKey = AuthSettings.DevelopmentSigningKey;
}
builder.Services.Configure<AuthSettings>(options =>
{
    options.AllowedDomain = authSettings.AllowedDomain;
    options.Issuer = authSettings.Issuer;
    options.Audience = authSettings.Audience;
    options.SigningKey = authSettings.SigningKey;
});

if (authSettings.SigningKey.Length < 32)
{
    throw new InvalidOperationException("Auth signing key must be at least 32 characters.");
}

if (builder.Environment.IsProduction() && authSettings.SigningKey == AuthSettings.DevelopmentSigningKey)
{
    throw new InvalidOperationException("Production requires a non-default Auth:SigningKey value.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = authSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = ClaimTypes.Email
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthorizationPolicies.AllowedDomainUser, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => IsAllowedDomain(GetEmailClaim(context.User), authSettings.AllowedDomain));
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    await next();
});
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    Service = "CycleSync.Api",
    Status = "Ready"
}));

app.MapPost("/api/auth/token", (TokenRequest request, IOptions<AuthSettings> options) =>
{
    if (string.IsNullOrWhiteSpace(request.Email))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["email"] = ["Email is required."]
        });
    }

    var authOptions = options.Value;
    if (!IsAllowedDomain(request.Email, authOptions.AllowedDomain))
    {
        return Results.Json(new { error = "Email domain is not allowed." }, statusCode: StatusCodes.Status403Forbidden);
    }

    var now = DateTimeOffset.UtcNow;
    var expiresAt = now.AddHours(1);
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SigningKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, request.Email),
        new Claim(ClaimTypes.Email, request.Email),
        new Claim(ClaimTypes.Name, request.Email),
        new Claim("preferred_username", request.Email)
    };

    var tokenDescriptor = new JwtSecurityToken(
        issuer: authOptions.Issuer,
        audience: authOptions.Audience,
        claims: claims,
        notBefore: now.UtcDateTime,
        expires: expiresAt.UtcDateTime,
        signingCredentials: credentials);

    var encodedToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    return Results.Ok(new TokenResponse(encodedToken, "Bearer", (int)TimeSpan.FromHours(1).TotalSeconds, expiresAt));
}).AllowAnonymous();

var apiGroup = app.MapGroup("/api")
    .RequireAuthorization(AuthorizationPolicies.AllowedDomainUser);

apiGroup.MapGet("/me", (ClaimsPrincipal user) =>
{
    var email = GetEmailClaim(user);
    return Results.Ok(new
    {
        email
    });
});

apiGroup.MapGet("/weatherforecast", (IMemoryCache cache, HttpContext context) =>
{
    var forecasts = cache.GetOrCreate("weather-forecast", entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        return BuildForecast(DateOnly.FromDateTime(DateTime.UtcNow));
    });

    context.Response.Headers[HeaderNames.CacheControl] = "private,max-age=30";
    return Results.Ok(forecasts);
});

app.MapDefaultEndpoints();

app.Run();

static bool IsAllowedDomain(string? email, string allowedDomain)
{
    if (string.IsNullOrWhiteSpace(email))
    {
        return false;
    }

    var atIndex = email.LastIndexOf('@');
    if (atIndex <= 0 || atIndex >= email.Length - 1)
    {
        return false;
    }

    var domain = email[(atIndex + 1)..];
    return domain.Equals(allowedDomain, StringComparison.OrdinalIgnoreCase);
}

static string? GetEmailClaim(ClaimsPrincipal principal)
{
    return principal.FindFirstValue(ClaimTypes.Email)
           ?? principal.FindFirstValue("preferred_username")
           ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
}

static IReadOnlyList<WeatherForecast> BuildForecast(DateOnly startDate)
{
    var summaries = new[]
    {
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    };

    return Enumerable.Range(1, 5)
        .Select(index =>
        {
            var temperatureC = Random.Shared.Next(-20, 45);
            return new WeatherForecast(
                startDate.AddDays(index),
                temperatureC,
                summaries[Random.Shared.Next(summaries.Length)]);
        })
        .ToArray();
}

internal sealed record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal sealed record TokenRequest(string Email);
internal sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresIn, DateTimeOffset ExpiresAtUtc);

internal static class AuthorizationPolicies
{
    public const string AllowedDomainUser = "AllowedDomainUser";
}

internal sealed class AuthSettings
{
    public const string SectionName = "Auth";
    public const string DevelopmentSigningKey = "dev-only-signing-key-change-in-production-2026";

    public string AllowedDomain { get; set; } = "contoso.com";
    public string Issuer { get; set; } = "CycleSync.Api";
    public string Audience { get; set; } = "CycleSync.Web";
    public string SigningKey { get; set; } = string.Empty;
}
