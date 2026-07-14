using CycleSync.Specs.Support;
using FluentAssertions;
using Reqnroll;

namespace CycleSync.Specs.Steps;

[Binding]
public sealed class CycleSyncStepDefinitions(ScenarioContext scenarioContext)
{
    private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
    private static readonly HashSet<string> AllowedAttendanceStatuses = new(Comparer)
    {
        "Interested",
        "Can't Make It",
        "Probably Coming",
        "Definitely Coming",
        "Booked"
    };

    private readonly Dictionary<string, UserProfile> _profiles = new(Comparer);
    private readonly HashSet<string> _authenticatedUsers = new(Comparer);
    private readonly HashSet<string> _savedLocations = new(Comparer);
    private readonly Dictionary<string, int> _interestCounts = new(Comparer);
    private readonly List<OffCycle> _offCycles = [];
    private readonly Dictionary<Guid, Dictionary<string, string>> _attendanceByOffCycle = [];
    private readonly List<CostEstimate> _latestEstimates = [];
    private readonly List<string> _searchResults = [];
    private readonly List<UserProfile> _visibleProfiles = [];
    private readonly List<OffCycle> _visibleOffCycles = [];

    private string? _requestedAuthenticationEmail;
    private bool _isAuthenticationSuccessful;
    private string? _currentUserEmail;
    private string? _secondUserEmail;
    private OffCycle? _activeOffCycle;
    private IReadOnlyDictionary<string, int>? _attendanceSummaryBeforeChange;
    private IReadOnlyDictionary<string, int>? _attendanceSummaryAfterChange;
    private bool _costEstimatesRecalculated;

    [Given("an authentication request for {string}")]
    public void GivenAnAuthenticationRequestFor(string email)
    {
        _requestedAuthenticationEmail = email;
    }

    [Given("an authenticated user profile for {string}")]
    public void GivenAnAuthenticatedUserProfileFor(string email)
    {
        GivenAnAuthenticatedUser(email);
        EnsureProfile(email);
    }

    [Given("a profile exists for {string}")]
    public void GivenAProfileExistsFor(string email)
    {
        EnsureProfile(email);
    }

    [Given("another authenticated user {string}")]
    public void GivenAnotherAuthenticatedUser(string email)
    {
        _secondUserEmail = email;
        _authenticatedUsers.Add(email);
    }

    [Given("an authenticated user {string}")]
    public void GivenAnAuthenticatedUser(string email)
    {
        _currentUserEmail = email;
        _authenticatedUsers.Add(email);
        EnsureProfile(email);
    }

    [Given("a saved location {string}")]
    public void GivenASavedLocation(string location)
    {
        _savedLocations.Add(location);
        _interestCounts.TryAdd(location, 0);
    }

    [Given("saved locations with interest totals")]
    public void GivenSavedLocationsWithInterestTotals(Table table)
    {
        foreach (var row in table.Rows)
        {
            var location = row["Location"];
            var count = int.Parse(row["InterestCount"]);
            _savedLocations.Add(location);
            _interestCounts[location] = count;
        }
    }

    [Given("an existing off-cycle in {string} from {string} to {string}")]
    public void GivenAnExistingOffCycleInFromTo(string location, string startDate, string endDate)
    {
        GivenASavedLocation(location);
        _activeOffCycle = new OffCycle(Guid.NewGuid(), location, ParseDate(startDate), ParseDate(endDate));
        _offCycles.Add(_activeOffCycle);
    }

    [Given("attendance statuses exist for that off-cycle")]
    public void GivenAttendanceStatusesExistForThatOffCycle()
    {
        var offCycle = RequireActiveOffCycle();
        _attendanceByOffCycle[offCycle.Id] = new Dictionary<string, string>(Comparer)
        {
            ["alex@contoso.com"] = "Interested",
            ["sam@contoso.com"] = "Probably Coming"
        };
    }

    [When("the user attempts to sign in")]
    public void WhenTheUserAttemptsToSignIn()
    {
        _requestedAuthenticationEmail.Should().NotBeNullOrWhiteSpace();
        _isAuthenticationSuccessful = IsAllowedDomain(_requestedAuthenticationEmail!);

        if (_isAuthenticationSuccessful)
        {
            _authenticatedUsers.Add(_requestedAuthenticationEmail!);
            _currentUserEmail = _requestedAuthenticationEmail;
            EnsureProfile(_requestedAuthenticationEmail!);
        }
    }

    [When("they update preferred currency to {string} and passports to {string}")]
    public void WhenTheyUpdatePreferredCurrencyToAndPassportsTo(string currency, string passports)
    {
        var email = RequireCurrentUser();
        var profile = EnsureProfile(email);
        profile.PreferredCurrency = currency;
        profile.Passports = passports;
    }

    [When("the second user views profiles")]
    public void WhenTheSecondUserViewsProfiles()
    {
        _secondUserEmail.Should().NotBeNullOrWhiteSpace();
        _authenticatedUsers.Should().Contain(_secondUserEmail!);
        _visibleProfiles.Clear();
        _visibleProfiles.AddRange(_profiles.Values);
    }

    [When("they search locations for {string}")]
    public void WhenTheySearchLocationsFor(string locationQuery)
    {
        RequireCurrentUser();
        _searchResults.Clear();

        var locations = _savedLocations.Count == 0
            ? new[] { "Lisbon", "Singapore", "Tokyo" }
            : _savedLocations.ToArray();

        _searchResults.AddRange(
            locations.Where(location =>
                location.Contains(locationQuery, StringComparison.OrdinalIgnoreCase)));
    }

    [When("they mark interest in {string}")]
    public void WhenTheyMarkInterestIn(string location)
    {
        RequireCurrentUser();
        _savedLocations.Should().Contain(location);
        _interestCounts.TryAdd(location, 0);
        _interestCounts[location] += 1;
    }

    [When("users request ranked locations")]
    public void WhenUsersRequestRankedLocations()
    {
        _searchResults.Clear();
        _searchResults.AddRange(
            _interestCounts
                .OrderByDescending(entry => entry.Value)
                .ThenBy(entry => entry.Key, Comparer)
                .Select(entry => entry.Key));
    }

    [When("they create an off-cycle in {string} from {string} to {string}")]
    public void WhenTheyCreateAnOffCycleInFromTo(string location, string startDate, string endDate)
    {
        RequireCurrentUser();
        _savedLocations.Should().Contain(location);

        _activeOffCycle = new OffCycle(Guid.NewGuid(), location, ParseDate(startDate), ParseDate(endDate));
        _offCycles.Add(_activeOffCycle);
    }

    [When("the second user views off-cycles")]
    public void WhenTheSecondUserViewsOffCycles()
    {
        _secondUserEmail.Should().NotBeNullOrWhiteSpace();
        _authenticatedUsers.Should().Contain(_secondUserEmail!);
        _visibleOffCycles.Clear();
        _visibleOffCycles.AddRange(_offCycles);
    }

    [When("they set attendance status to {string}")]
    public void WhenTheySetAttendanceStatusTo(string status)
    {
        var offCycle = RequireActiveOffCycle();
        var user = RequireCurrentUser();
        AllowedAttendanceStatuses.Should().Contain(status);
        var statuses = EnsureAttendanceState(offCycle.Id);
        statuses[user] = status;
    }

    [When("a user changes attendance status from {string} to {string}")]
    public void WhenAUserChangesAttendanceStatusFromTo(string originalStatus, string updatedStatus)
    {
        var offCycle = RequireActiveOffCycle();
        var user = "alex@contoso.com";
        var statuses = EnsureAttendanceState(offCycle.Id);
        statuses.Should().ContainKey(user);
        statuses[user].Should().Be(originalStatus);

        _attendanceSummaryBeforeChange = BuildAttendanceSummary(statuses);
        statuses[user] = updatedStatus;
        _attendanceSummaryAfterChange = BuildAttendanceSummary(statuses);
    }

    [When("a user requests cost estimates")]
    public void WhenAUserRequestsCostEstimates()
    {
        GenerateEstimates(RequireActiveOffCycle(), isRecalculation: false);
    }

    [When("off-cycle dates change to {string} and {string}")]
    public void WhenOffCycleDatesChangeToAnd(string startDate, string endDate)
    {
        var offCycle = RequireActiveOffCycle();
        offCycle.StartDate = ParseDate(startDate);
        offCycle.EndDate = ParseDate(endDate);
        GenerateEstimates(offCycle, isRecalculation: true);
    }

    [Then("the user should be authenticated")]
    public void ThenTheUserShouldBeAuthenticated()
    {
        _isAuthenticationSuccessful.Should().BeTrue();
    }

    [Then("the sign-in should be rejected")]
    public void ThenTheSignInShouldBeRejected()
    {
        _isAuthenticationSuccessful.Should().BeFalse();
    }

    [Then("the profile should store preferred currency {string} and passports {string}")]
    public void ThenTheProfileShouldStorePreferredCurrencyAndPassports(string currency, string passports)
    {
        var profile = EnsureProfile(RequireCurrentUser());
        profile.PreferredCurrency.Should().Be(currency);
        profile.Passports.Should().Be(passports);
    }

    [Then("the profile for {string} should be visible")]
    public void ThenTheProfileForShouldBeVisible(string email)
    {
        _visibleProfiles.Should().Contain(profile => Comparer.Equals(profile.Email, email));
    }

    [Then("the search results should include {string}")]
    public void ThenTheSearchResultsShouldInclude(string location)
    {
        _searchResults.Should().Contain(result => Comparer.Equals(result, location));
    }

    [Then("interest count for {string} should be {int}")]
    public void ThenInterestCountForShouldBe(string location, int interestCount)
    {
        _interestCounts.Should().ContainKey(location);
        _interestCounts[location].Should().Be(interestCount);
    }

    [Then("locations should be ranked by interest count descending")]
    public void ThenLocationsShouldBeRankedByInterestCountDescending()
    {
        _searchResults.Should().ContainInOrder("Singapore", "Tokyo", "Lisbon");
    }

    [Then("the off-cycle should be created for {string} from {string} to {string}")]
    public void ThenTheOffCycleShouldBeCreatedForFromTo(string location, string startDate, string endDate)
    {
        var expectedStartDate = ParseDate(startDate);
        var expectedEndDate = ParseDate(endDate);
        _offCycles.Should().Contain(offCycle =>
            Comparer.Equals(offCycle.Location, location)
            && offCycle.StartDate == expectedStartDate
            && offCycle.EndDate == expectedEndDate);
    }

    [Then("the off-cycle in {string} should be visible")]
    public void ThenTheOffCycleInShouldBeVisible(string location)
    {
        _visibleOffCycles.Should().Contain(offCycle => Comparer.Equals(offCycle.Location, location));
    }

    [Then("attendance status should be {string}")]
    public void ThenAttendanceStatusShouldBe(string status)
    {
        var offCycle = RequireActiveOffCycle();
        var user = RequireCurrentUser();
        var statuses = EnsureAttendanceState(offCycle.Id);
        statuses[user].Should().Be(status);
    }

    [Then("the off-cycle summary counts should be updated")]
    public void ThenTheOffCycleSummaryCountsShouldBeUpdated()
    {
        _attendanceSummaryBeforeChange.Should().NotBeNull();
        _attendanceSummaryAfterChange.Should().NotBeNull();
        _attendanceSummaryBeforeChange.Should().NotBeEquivalentTo(_attendanceSummaryAfterChange);
    }

    [Then("each estimate should include confidence and generated timestamp")]
    public void ThenEachEstimateShouldIncludeConfidenceAndGeneratedTimestamp()
    {
        _latestEstimates.Should().NotBeEmpty();
        _latestEstimates.Should().OnlyContain(estimate =>
            estimate.Confidence > 0
            && estimate.GeneratedAtUtc != default
            && estimate.StartDate != default
            && estimate.EndDate != default);
    }

    [Then("cost estimates should be recalculated for the new dates")]
    public void ThenCostEstimatesShouldBeRecalculatedForTheNewDates()
    {
        var offCycle = RequireActiveOffCycle();
        _costEstimatesRecalculated.Should().BeTrue();
        _latestEstimates.Should().OnlyContain(estimate =>
            estimate.StartDate == offCycle.StartDate
            && estimate.EndDate == offCycle.EndDate);
    }

    private static bool IsAllowedDomain(string email)
    {
        var atIndex = email.LastIndexOf('@');
        return atIndex > 0
               && atIndex < email.Length - 1
               && email[(atIndex + 1)..].Equals("contoso.com", StringComparison.OrdinalIgnoreCase);
    }

    private static DateOnly ParseDate(string value) => DateOnly.ParseExact(value, "yyyy-MM-dd");

    private string RequireCurrentUser()
    {
        _currentUserEmail.Should().NotBeNullOrWhiteSpace();
        _authenticatedUsers.Should().Contain(_currentUserEmail!);
        return _currentUserEmail!;
    }

    private OffCycle RequireActiveOffCycle()
    {
        _activeOffCycle.Should().NotBeNull();
        return _activeOffCycle!;
    }

    private UserProfile EnsureProfile(string email)
    {
        if (_profiles.TryGetValue(email, out var existing))
        {
            return existing;
        }

        var profile = new UserProfile(email, "EUR", string.Empty);
        _profiles[email] = profile;
        return profile;
    }

    private Dictionary<string, string> EnsureAttendanceState(Guid offCycleId)
    {
        if (_attendanceByOffCycle.TryGetValue(offCycleId, out var statuses))
        {
            return statuses;
        }

        statuses = new Dictionary<string, string>(Comparer);
        _attendanceByOffCycle[offCycleId] = statuses;
        return statuses;
    }

    private static IReadOnlyDictionary<string, int> BuildAttendanceSummary(Dictionary<string, string> statuses)
    {
        return statuses
            .GroupBy(entry => entry.Value, Comparer)
            .ToDictionary(group => group.Key, group => group.Count(), Comparer);
    }

    private void GenerateEstimates(OffCycle offCycle, bool isRecalculation)
    {
        _latestEstimates.Clear();
        _latestEstimates.Add(
            new CostEstimate(
                offCycle.Id,
                offCycle.StartDate,
                offCycle.EndDate,
                0.82m,
                (DateTimeOffset)scenarioContext[TestHooks.FixedClockNowUtcKey]));

        _costEstimatesRecalculated = isRecalculation;
    }

    private sealed record UserProfile(string Email, string PreferredCurrency, string Passports)
    {
        public string PreferredCurrency { get; set; } = PreferredCurrency;
        public string Passports { get; set; } = Passports;
    }

    private sealed record OffCycle(Guid Id, string Location, DateOnly StartDate, DateOnly EndDate)
    {
        public DateOnly StartDate { get; set; } = StartDate;
        public DateOnly EndDate { get; set; } = EndDate;
    }

    private sealed record CostEstimate(
        Guid OffCycleId,
        DateOnly StartDate,
        DateOnly EndDate,
        decimal Confidence,
        DateTimeOffset GeneratedAtUtc);
}
