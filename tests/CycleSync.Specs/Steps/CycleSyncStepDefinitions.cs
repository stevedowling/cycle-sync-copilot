using Reqnroll;

namespace CycleSync.Specs.Steps;

[Binding]
public sealed class CycleSyncStepDefinitions
{
    [Given("an authentication request for {string}")]
    public void GivenAnAuthenticationRequestFor(string email)
    {
        Pending($"Authentication request setup for '{email}'.");
    }

    [Given("an authenticated user profile for {string}")]
    public void GivenAnAuthenticatedUserProfileFor(string email)
    {
        Pending($"Authenticated profile setup for '{email}'.");
    }

    [Given("a profile exists for {string}")]
    public void GivenAProfileExistsFor(string email)
    {
        Pending($"Existing profile setup for '{email}'.");
    }

    [Given("another authenticated user {string}")]
    public void GivenAnotherAuthenticatedUser(string email)
    {
        Pending($"Second authenticated user setup for '{email}'.");
    }

    [Given("an authenticated user {string}")]
    public void GivenAnAuthenticatedUser(string email)
    {
        Pending($"Authenticated user setup for '{email}'.");
    }

    [Given("a saved location {string}")]
    public void GivenASavedLocation(string location)
    {
        Pending($"Saved location setup for '{location}'.");
    }

    [Given("saved locations with interest totals")]
    public void GivenSavedLocationsWithInterestTotals(Table table)
    {
        Pending($"Seed location interest totals:\n{table}");
    }

    [Given("an existing off-cycle in {string} from {string} to {string}")]
    public void GivenAnExistingOffCycleInFromTo(string location, string startDate, string endDate)
    {
        Pending($"Existing off-cycle setup for '{location}' ({startDate}..{endDate}).");
    }

    [Given("attendance statuses exist for that off-cycle")]
    public void GivenAttendanceStatusesExistForThatOffCycle()
    {
        Pending("Seed attendance statuses for summary assertions.");
    }

    [When("the user attempts to sign in")]
    public void WhenTheUserAttemptsToSignIn()
    {
        Pending("Execute sign-in flow.");
    }

    [When("they update preferred currency to {string} and passports to {string}")]
    public void WhenTheyUpdatePreferredCurrencyToAndPassportsTo(string currency, string passports)
    {
        Pending($"Update profile currency '{currency}' and passports '{passports}'.");
    }

    [When("the second user views profiles")]
    public void WhenTheSecondUserViewsProfiles()
    {
        Pending("List all visible profiles.");
    }

    [When("they search locations for {string}")]
    public void WhenTheySearchLocationsFor(string locationQuery)
    {
        Pending($"Search locations for '{locationQuery}'.");
    }

    [When("they mark interest in {string}")]
    public void WhenTheyMarkInterestIn(string location)
    {
        Pending($"Mark interest in '{location}'.");
    }

    [When("users request ranked locations")]
    public void WhenUsersRequestRankedLocations()
    {
        Pending("Request ranked locations.");
    }

    [When("they create an off-cycle in {string} from {string} to {string}")]
    public void WhenTheyCreateAnOffCycleInFromTo(string location, string startDate, string endDate)
    {
        Pending($"Create off-cycle '{location}' ({startDate}..{endDate}).");
    }

    [When("the second user views off-cycles")]
    public void WhenTheSecondUserViewsOffCycles()
    {
        Pending("List visible off-cycles.");
    }

    [When("they set attendance status to {string}")]
    public void WhenTheySetAttendanceStatusTo(string status)
    {
        Pending($"Set attendance status '{status}'.");
    }

    [When("a user changes attendance status from {string} to {string}")]
    public void WhenAUserChangesAttendanceStatusFromTo(string originalStatus, string updatedStatus)
    {
        Pending($"Change attendance status from '{originalStatus}' to '{updatedStatus}'.");
    }

    [When("a user requests cost estimates")]
    public void WhenAUserRequestsCostEstimates()
    {
        Pending("Request cost estimates.");
    }

    [When("off-cycle dates change to {string} and {string}")]
    public void WhenOffCycleDatesChangeToAnd(string startDate, string endDate)
    {
        Pending($"Change off-cycle dates to {startDate}..{endDate}.");
    }

    [Then("the user should be authenticated")]
    public void ThenTheUserShouldBeAuthenticated()
    {
        Pending("Assert successful authentication.");
    }

    [Then("the sign-in should be rejected")]
    public void ThenTheSignInShouldBeRejected()
    {
        Pending("Assert sign-in rejection.");
    }

    [Then("the profile should store preferred currency {string} and passports {string}")]
    public void ThenTheProfileShouldStorePreferredCurrencyAndPassports(string currency, string passports)
    {
        Pending($"Assert profile persisted currency '{currency}' and passports '{passports}'.");
    }

    [Then("the profile for {string} should be visible")]
    public void ThenTheProfileForShouldBeVisible(string email)
    {
        Pending($"Assert profile visibility for '{email}'.");
    }

    [Then("the search results should include {string}")]
    public void ThenTheSearchResultsShouldInclude(string location)
    {
        Pending($"Assert search includes '{location}'.");
    }

    [Then("interest count for {string} should be {int}")]
    public void ThenInterestCountForShouldBe(string location, int interestCount)
    {
        Pending($"Assert interest count for '{location}' is {interestCount}.");
    }

    [Then("locations should be ranked by interest count descending")]
    public void ThenLocationsShouldBeRankedByInterestCountDescending()
    {
        Pending("Assert location ranking by interest count descending.");
    }

    [Then("the off-cycle should be created for {string} from {string} to {string}")]
    public void ThenTheOffCycleShouldBeCreatedForFromTo(string location, string startDate, string endDate)
    {
        Pending($"Assert off-cycle created for '{location}' ({startDate}..{endDate}).");
    }

    [Then("the off-cycle in {string} should be visible")]
    public void ThenTheOffCycleInShouldBeVisible(string location)
    {
        Pending($"Assert off-cycle visibility for '{location}'.");
    }

    [Then("attendance status should be {string}")]
    public void ThenAttendanceStatusShouldBe(string status)
    {
        Pending($"Assert attendance status is '{status}'.");
    }

    [Then("the off-cycle summary counts should be updated")]
    public void ThenTheOffCycleSummaryCountsShouldBeUpdated()
    {
        Pending("Assert off-cycle summary counts were updated.");
    }

    [Then("each estimate should include confidence and generated timestamp")]
    public void ThenEachEstimateShouldIncludeConfidenceAndGeneratedTimestamp()
    {
        Pending("Assert estimates include confidence and generated timestamp.");
    }

    [Then("cost estimates should be recalculated for the new dates")]
    public void ThenCostEstimatesShouldBeRecalculatedForTheNewDates()
    {
        Pending("Assert estimates are recalculated for updated date range.");
    }

    private static void Pending(string context)
    {
        throw new NotImplementedException($"Phase 1 pending step: {context}");
    }
}
