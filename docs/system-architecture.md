# CycleSync System Architecture (React + .NET + SQL Server + Aspire)

## 1. Runtime Topology

```text
[React SPA] --> [CycleSync API (.NET)] --> [SQL Server]
                    |        |
                    |        +--> [Azure Maps]
                    +----------> [AI Intelligence Provider]

All services orchestrated by .NET Aspire AppHost.
```

## 2. Backend Service Boundaries

| Module | Responsibility |
|---|---|
| Identity | OAuth sign-in, domain restriction, user bootstrap |
| Profiles | User preferences, home location, passports, language/currency |
| Locations | Search, persistent location records, intelligence summary |
| Interests | User-location interest tracking and aggregate counts |
| OffCycles | Event lifecycle, dates, linked location |
| Attendance | Per-user attendance status per off-cycle |
| Costs | Estimated travel/accommodation/daily costs with confidence metadata |

## 3. Initial Data Model (SQL Server)

| Table | Purpose |
|---|---|
| `Users` | Auth subject + profile attributes |
| `Passports` | Many-to-one user nationality/passport records |
| `Locations` | Persistent destinations (never deleted) |
| `LocationInterests` | User interest in locations |
| `OffCycles` | Planned events with date ranges |
| `AttendanceStatuses` | User attendance per off-cycle |
| `CostEstimates` | Location/off-cycle cost estimates with confidence/timestamp |

**Key rule:** locations are soft-retained permanently (no hard delete flow in application behavior).

## 4. API Contract Shape

1. `/api/me` and `/api/me/profile` for current user data.
2. `/api/locations/search` for Azure Maps-driven discovery.
3. `/api/locations/{id}/interest` for interest toggling.
4. `/api/offcycles` CRUD endpoints.
5. `/api/offcycles/{id}/attendance` for status updates.
6. `/api/cost-estimates` for estimate retrieval and recomputation.

## 5. Frontend SPA Composition

| Route | Purpose |
|---|---|
| `/` | Location discovery, interest sorting, intelligence cards |
| `/profile` | User profile preferences |
| `/offcycles` | Off-cycle list/create |
| `/offcycles/:id` | Off-cycle details + attendance + recalculated costs |

## 6. Aspire Composition Plan

1. `CycleSync.AppHost` provisions SQL Server resource.
2. API project receives SQL connection via Aspire service discovery.
3. SPA and API run together in local distributed app environment.
4. Telemetry (logs/traces/metrics) enabled through Aspire defaults.

## 7. Cross-Cutting Rules

1. All authenticated users have equal permissions.
2. Every estimate returned to clients includes confidence and generation time.
3. Data is visible to all authenticated users within the allowed domain.
