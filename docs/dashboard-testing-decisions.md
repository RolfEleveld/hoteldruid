# Dashboard Testing Decisions

## Scope

This document captures decisions for validating dashboard business flow behavior with integration-style UI tests.

Business flow sequence under test:

1. Reservations
2. Arrivals
3. Stays
4. Departures

## Why bUnit Integration Tests

We use bUnit-backed integration tests in `tests/HotelDruid.Client.Tests` to validate rendered UI behavior with mocked API responses because they:

- run fast in CI and local development,
- verify user-visible behavior without brittle browser automation,
- allow deterministic scenario data for lifecycle transitions.

## Visual Validation Strategy

"Visual tests" are implemented as **rendered-structure assertions** rather than pixel snapshots.

What is validated:

- lifecycle widget presence and order,
- row menu availability and action sets per widget,
- settings/menu alignment classes for submenu placement,
- behavior influenced by system settings (for example arrivals look-ahead days).

This is intentional to keep tests robust when minor CSS tweaks occur.

## Flexibility Rules

To keep tests stable through minor UI changes:

- assert semantic markers (`title` keys, widget titles, action labels) before exact markup,
- avoid strict full-markup snapshots,
- avoid over-constraining item counts unless business-critical,
- prefer scenario-focused data setup (`today +/- N days`) over fixed dates.

## Current Suite

Primary suite:

- `tests/HotelDruid.Client.Tests/DashboardBusinessFlowIntegrationTests.cs`

Coverage includes:

- flow ordering validation,
- lifecycle classification (reservations/arrivals/stays/departures),
- business action menus for each lifecycle stage,
- submenu alignment expectations,
- arrivals window setting behavior.

## Future Extensions

If browser-level visual regression is needed later, add Playwright snapshots as a separate layer. Keep bUnit integration tests as the primary business-process regression safety net.
