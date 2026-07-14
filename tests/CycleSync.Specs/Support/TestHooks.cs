using Reqnroll;

namespace CycleSync.Specs.Support;

[Binding]
public sealed class TestHooks(ScenarioContext scenarioContext)
{
    public const string FixedClockNowUtcKey = "FixedClockNowUtc";

    [BeforeScenario(Order = 0)]
    public void ResetScenarioState()
    {
        // Fixed clock keeps scenarios deterministic while implementation is added in later phases.
        scenarioContext[FixedClockNowUtcKey] = new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero);
    }
}
