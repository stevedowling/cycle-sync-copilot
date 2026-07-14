# CycleSync SPA Implementation Plan

This plan defines how to deliver CycleSync as a single-page application using **React**, **C# .NET**, **SQL Server**, and **.NET Aspire**, starting with **BDD tests**.

## 1. Target Architecture

| Layer | Technology | Responsibility |
|---|---|---|
| Frontend SPA | React + TypeScript + Vite | UI, routing, auth session, API integration |
| Backend API | ASP.NET Core (.NET 10) | Auth, business rules, location/off-cycle/attendance APIs |
| Data | SQL Server + EF Core | Persistent storage and migrations |
| Orchestration | .NET Aspire AppHost | Local dev composition, service discovery, observability |
| Testing | Reqnroll (Gherkin) + xUnit + Playwright | BDD acceptance tests (API first, UI next) |

## 2. Proposed Solution Structure

```text
src/
  CycleSync.AppHost/          # Aspire orchestration project
  CycleSync.ServiceDefaults/  # Shared Aspire defaults
  CycleSync.Api/              # ASP.NET Core Web API
  CycleSync.Web/              # React SPA
tests/
  CycleSync.Specs/            # Reqnroll BDD specs
  CycleSync.Web.Specs/        # Playwright BDD UI specs (phase 2)
```

## 3. Delivery Phases (BDD First)

## Phase 0: Foundations
1. Create Aspire solution and projects.
2. Add SQL Server container/resource in AppHost.
3. Wire API and SPA into AppHost.
4. Configure baseline observability and health checks.

## Phase 1: BDD Specification (Before Implementation)
1. Define ubiquitous language and acceptance criteria in Gherkin.
2. Create `.feature` files for MVP capabilities.
3. Implement step definitions as failing tests first.
4. Establish test data/reset strategy for deterministic runs.

## Phase 2: Vertical Slices (Red-Green-Refactor)
1. Authentication and user profile.
2. Location search and saved location records.
3. Interest tracking and ranking.
4. Off-cycle creation and attendance status.
5. Cost estimate generation and transparency metadata.

Each slice starts by extending BDD scenarios, then implementing API, data model, and SPA behavior to pass.

## Phase 3: Hardening
1. Security checks (auth boundaries, token validation, domain restriction).
2. Performance checks for top flows.
3. Accessibility pass on core screens.
4. Production config and deployment pipeline.

## 4. MVP Feature-to-Slice Mapping

| MVP Capability | First Deliverable Slice |
|---|---|
| Google OAuth (domain-restricted) | Auth + user bootstrap |
| User profile (location, currency, language, passports) | Profile management |
| Location search (Azure Maps) | Search endpoint + search UI |
| AI location intelligence | Intelligence enrichment service |
| Interest tracking/sorting | Interest API + location ranking UI |
| Off-cycle create/manage | Off-cycle CRUD + details screen |
| Attendance status tracking | Attendance API + participant table |
| Cost estimation | Cost model service + UI badges/timestamps |

## 5. Definition of Done for Each Slice

1. BDD scenarios are green.
2. API contracts are versioned and documented.
3. EF migrations are generated and applied in local Aspire environment.
4. SPA supports success, loading, and error states.
5. Telemetry logs include correlation IDs for key operations.

## 6. Immediate Next Execution Order

1. Scaffold Aspire solution and projects.
2. Add initial BDD features for authentication, profile, and location interest.
3. Make tests executable in CI (even if red at first commit).
4. Implement first thin vertical slice: sign-in + profile read/update.
