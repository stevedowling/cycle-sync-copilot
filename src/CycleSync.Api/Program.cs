var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new
{
    Service = "CycleSync.Api",
    Status = "Ready"
}));

app.MapDefaultEndpoints();

app.Run();
