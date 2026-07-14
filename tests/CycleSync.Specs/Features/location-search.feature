@phase1 @mvp
Feature: Location search
  In order to discover meetup destinations
  As an authenticated teammate
  I want to search locations

  Scenario: user searches for a location by name
    Given an authenticated user "alex@contoso.com"
    When they search locations for "Lisbon"
    Then the search results should include "Lisbon"
