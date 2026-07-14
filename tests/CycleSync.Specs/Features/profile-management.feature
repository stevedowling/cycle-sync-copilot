@phase1 @mvp
Feature: Profile management
  In order to keep travel preferences current
  As an authenticated teammate
  I want to update and share my profile details

  Scenario: authenticated user updates preferred currency and passports
    Given an authenticated user profile for "alex@contoso.com"
    When they update preferred currency to "USD" and passports to "US,IE"
    Then the profile should store preferred currency "USD" and passports "US,IE"

  Scenario: profile is visible to all authenticated users
    Given a profile exists for "alex@contoso.com"
    And another authenticated user "sam@contoso.com"
    When the second user views profiles
    Then the profile for "alex@contoso.com" should be visible
