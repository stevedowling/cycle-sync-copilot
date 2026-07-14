@phase1 @mvp
Feature: Authentication
  In order to access CycleSync securely
  As a teammate
  I want authentication to enforce allowed company domains

  Scenario: user signs in with allowed company domain
    Given an authentication request for "alex@contoso.com"
    When the user attempts to sign in
    Then the user should be authenticated

  Scenario: user is rejected when domain is not allowed
    Given an authentication request for "alex@gmail.com"
    When the user attempts to sign in
    Then the sign-in should be rejected
