@phase1 @mvp
Feature: Cost estimation
  In order to plan realistic budgets
  As a teammate
  I want cost estimates with transparency metadata

  Scenario: cost estimate includes confidence and generated timestamp
    Given an existing off-cycle in "Lisbon" from "2026-10-10" to "2026-10-14"
    When a user requests cost estimates
    Then each estimate should include confidence and generated timestamp

  Scenario: estimates recalculate for selected off-cycle dates
    Given an existing off-cycle in "Lisbon" from "2026-10-10" to "2026-10-14"
    When off-cycle dates change to "2026-11-01" and "2026-11-05"
    Then cost estimates should be recalculated for the new dates
