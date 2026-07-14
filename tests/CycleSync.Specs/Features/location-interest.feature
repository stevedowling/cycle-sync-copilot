@phase1 @mvp
Feature: Location interest
  In order to find consensus destinations
  As a teammate
  I want to track and rank location interest

  Scenario: user marks location as interested
    Given an authenticated user "alex@contoso.com"
    And a saved location "Lisbon"
    When they mark interest in "Lisbon"
    Then interest count for "Lisbon" should be 1

  Scenario: location ranking orders by total interest count
    Given saved locations with interest totals
      | Location  | InterestCount |
      | Lisbon    | 3             |
      | Singapore | 5             |
      | Tokyo     | 4             |
    When users request ranked locations
    Then locations should be ranked by interest count descending
