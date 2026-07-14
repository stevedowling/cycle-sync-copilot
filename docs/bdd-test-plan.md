# CycleSync BDD Test Plan

CycleSync delivery starts with executable behavior specifications using **Gherkin** and **Reqnroll**.

## 1. Tooling

| Area | Tooling |
|---|---|
| BDD framework | Reqnroll + xUnit |
| Assertions | FluentAssertions |
| API host for specs | ASP.NET Core `WebApplicationFactory` |
| Test database | SQL Server test DB (containerized) |
| UI BDD (phase 2) | Playwright + Gherkin layer |

## 2. Test Pyramid for MVP

1. **Acceptance (BDD/API)**: primary source of truth for behavior.
2. **Integration**: repository/external integration (Azure Maps, AI service stubs).
3. **Unit**: domain rules (attendance transitions, cost model calculations).

## 3. Initial Feature Files

Create these in `tests/CycleSync.Specs/Features`:

1. `authentication.feature`
2. `profile-management.feature`
3. `location-search.feature`
4. `location-interest.feature`
5. `offcycle-management.feature`
6. `attendance-status.feature`
7. `cost-estimation.feature`

## 4. Seed Scenarios (First Wave)

## authentication.feature
- Scenario: user signs in with allowed company domain
- Scenario: user is rejected when domain is not allowed

## profile-management.feature
- Scenario: authenticated user updates preferred currency and passports
- Scenario: profile is visible to all authenticated users

## location-interest.feature
- Scenario: user marks location as interested
- Scenario: location ranking orders by total interest count

## offcycle-management.feature
- Scenario: user creates off-cycle with location and date range
- Scenario: off-cycle remains visible for all users

## attendance-status.feature
- Scenario Outline: user sets attendance status (`Interested`, `Can't Make It`, `Probably Coming`, `Definitely Coming`, `Booked`)
- Scenario: changing status updates off-cycle summary counts

## cost-estimation.feature
- Scenario: cost estimate includes confidence and generated timestamp
- Scenario: estimates recalculate for selected off-cycle dates

## 5. Step Definition Strategy

1. Use API-level steps first (`Given an authenticated user`, `When they create an off-cycle`, `Then attendance count is ...`).
2. Keep steps domain-focused, not transport-focused.
3. Reuse shared context objects for users, locations, and off-cycles.
4. Stub third-party APIs in BDD runs unless the scenario explicitly validates provider integration.

## 6. Test Data and Isolation

1. Reset database per scenario (transaction rollback or schema reset).
2. Use deterministic test fixtures for users and locations.
3. Keep date/time deterministic with injectable clock.

## 7. CI Gates (Order)

1. Run unit tests.
2. Run integration tests.
3. Run BDD API specs.
4. Run UI BDD specs (after core API slices are stable).
