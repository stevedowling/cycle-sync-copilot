@phase1 @mvp
Feature: Attendance status
  In order to understand participation confidence
  As a teammate
  I want to set attendance statuses for an off-cycle

  Scenario Outline: user sets attendance status
    Given an existing off-cycle in "Lisbon" from "2026-10-10" to "2026-10-14"
    And an authenticated user "alex@contoso.com"
    When they set attendance status to "<Status>"
    Then attendance status should be "<Status>"

    Examples:
      | Status           |
      | Interested       |
      | Can't Make It    |
      | Probably Coming  |
      | Definitely Coming |
      | Booked           |

  Scenario: changing status updates off-cycle summary counts
    Given an existing off-cycle in "Lisbon" from "2026-10-10" to "2026-10-14"
    And attendance statuses exist for that off-cycle
    When a user changes attendance status from "Interested" to "Booked"
    Then the off-cycle summary counts should be updated
