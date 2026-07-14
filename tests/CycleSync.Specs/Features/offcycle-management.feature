@phase1 @mvp
Feature: Off-cycle management
  In order to plan concrete meetup events
  As a teammate
  I want to create and view off-cycles

  Scenario: user creates off-cycle with location and date range
    Given an authenticated user "alex@contoso.com"
    And a saved location "Lisbon"
    When they create an off-cycle in "Lisbon" from "2026-10-10" to "2026-10-14"
    Then the off-cycle should be created for "Lisbon" from "2026-10-10" to "2026-10-14"

  Scenario: off-cycle remains visible for all users
    Given an existing off-cycle in "Lisbon" from "2026-10-10" to "2026-10-14"
    And another authenticated user "sam@contoso.com"
    When the second user views off-cycles
    Then the off-cycle in "Lisbon" should be visible
