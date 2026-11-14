# Booking Lifecycle Consistency (API-First)

## Goal
Define one canonical lifecycle model for booking classification so dashboard widgets, detail list pages, and action buttons are always consistent.

Classification must be derived from booking data fields (not stored as a separate persisted state), including imported data.

## Canonical States

| State | Meaning | Predicate (derived) |
|---|---|---|
| Reservation | Confirmed/active booking not yet arrived | `!Cancelled && ValidDates && !ActualArrivalDate` |
| Arrival | Reservation that is due now (or in look-ahead window) and not checked in | `Reservation && ArrivalDate in [today, today+lookAheadDays]` |
| Stay | Guest checked in and currently in-house | `!Cancelled && RoomId && ActualArrivalDate && ActualGuestCount>0 && ActualArrivalDate<=today && today<DepartureDate` |
| Departure | Guest should be checked out today (or overdue if configured) | `StayBase && DepartureDate==today` (or `<=today` if overdue mode) |
| Departed | Historical completed stay | `ActualArrivalDate && DepartureDate<today && CheckedOutAt` (if checkout timestamp introduced) |
| NoShow | Guest did not arrive by cutoff | `!Cancelled && !ActualArrivalDate && today>ArrivalDate && Status indicates no-show` |
| Cancelled | Booking removed from active lifecycle | `Status contains cancel/void/deleted OR moved to cancelled_bookings` |

Notes:
- `ValidDates` means `ArrivalDate` and `DepartureDate` exist and `DepartureDate > ArrivalDate`.
- `StayBase` means the stay structural requirements excluding the `today` window check.

## Checkout State Decision

`ActualDepartureDate` is stored on the booking. Check-out sets it and sets status to `CheckedOut`. A booking with that date leaves the stay and departure lists.

Still open, and optional:
- `CheckedOutByUserId` and `CheckedOutAt` are not stored. The status and `ActualDepartureDate` are the markers in use.

Why the checkout date is stored:
- Most PMS/hotel systems keep both planned departure and actual checkout.
- It removes ambiguity on departure day (guest still in room vs already checked out).
- It supports accurate operational dashboards, housekeeping, billing cutoff, and night-audit reporting.

Fallback when `ActualDepartureDate` is missing:
- Derive checkout state as unknown and use lifecycle heuristics:
	- If `ActualArrivalDate` exists and `today < DepartureDate` => Stay
	- If `ActualArrivalDate` exists and `today == DepartureDate` => Departure (expected checkout)
	- If `ActualArrivalDate` exists and `today > DepartureDate` => Overdue departure (not auto-checked-out)

State transition rule:
- `Check Out` action sets `ActualDepartureDate=today` (and audit metadata if present).
- A booking with `ActualDepartureDate` is considered departed and should no longer appear in Stay/Departure active lists.

## Maturity States (Tracking)

| Maturity Level | Description | Exit Criteria |
|---|---|---|
| M0 | Ad-hoc lifecycle logic in UI | Multiple rule copies and divergent results |
| M1 | Core state documented | Canonical state predicates documented and agreed |
| M2 | API derives all list states | Reservations/arrivals/stays/departures served by API-derived predicates |
| M3 | UI fully API-driven | Widgets and state-specific detail lists consume lifecycle endpoints only |
| M4 | API transition actions complete | Check-in/check-out/no-show/payment endpoints mutate data safely |
| M5 | Import and runtime parity verified | Imported data classification matches live classification rules |
| M6 | Regression safety net complete | API + client integration tests cover boundaries and transitions |

Current maturity snapshot:
- Lifecycle derivation: `M2`
- Widget/detail list consumption: `M3`
- Transition action endpoints: `M4` for check-in, check-out, and no-show. Payment is the ledger transaction endpoint, not a separate `/payment` route.
- Import parity: `M3` for field mapping (`ActualDepartureDate` is imported when the legacy source has a checkout). Classification tests that assert which list an imported booking appears in are still open, so this is not `M5`.
- Test coverage: `M4` for API state lists and the three transition endpoints (`Layer4ApiTests`). Client view routing is covered. A client test that clicks check-in, check-out, and no-show is still open, so this is not `M6`.

## Current Implementation Validation

| Surface | Current Data Source | Current Rule Owner | Status |
|---|---|---|---|
| Landing widget: Reservations | `/api/bookings/reservations` | API (`Program.cs`) | Centralized |
| Landing widget: Arrivals | `/api/bookings/arrivals?lookAheadDays=N` | API (`Program.cs`) | Centralized |
| Landing widget: Stays | `/api/bookings/stays` | API (`Program.cs`) | Centralized |
| Landing widget: Departures | `/api/bookings/departures` | API (`Program.cs`) | Centralized |
| Detail page left column `/bookings?view=reservations` | `/api/bookings/reservations` | API | Centralized |
| Detail page left column `/bookings?view=stays` | `/api/bookings/stays` | API | Centralized |
| Detail page left column `/bookings?view=arrivals` | `/api/bookings/arrivals?lookAheadDays=N` | API | Centralized |
| Detail page left column `/bookings?view=departures` | `/api/bookings/departures` | API | Centralized |
| Detail page left column `/bookings` | `/api/bookings` | Generic list | Expected |

## Required Button Sets by Detail State

| Detail State Page | Required Actions | Data Auto-Set Rules |
|---|---|---|
| Reservation | Save, Cancel, Cancel Booking, Delete | Save validates core fields. Cancel Booking moves to cancelled archive. |
| Arrival | Arrival, Check In, Payment, No-Show | `Check In` sets `ActualArrivalDate=today`, `ActualGuestCount=1 if missing`, keeps room/client/year, status defaults to `CheckedIn` if empty. `No-Show` sets status and removes from active arrival pipeline. |
| Stay | Check Out, Payment | `Check Out` sets checkout marker (new field recommended: `ActualDepartureDate=today`) and/or status to `CheckedOut`; removes from active stay list. |
| Departure | Check Out, Payment | Same checkout behavior as Stay; if overdue mode enabled, still available after departure date until checked out. |

## API Contract (Target)

Prefer one generic endpoint:

- `GET /api/bookings/lifecycle?state=reservation|arrival|stay|departure&year=YYYY&lookAheadDays=N`

Alternative (equivalent):
- `GET /api/bookings/reservations`
- `GET /api/bookings/arrivals`
- `GET /api/bookings/stays`
- `GET /api/bookings/departures`

Action endpoints:
- `POST /api/bookings/{year}/{id}/check-in`
- `POST /api/bookings/{year}/{id}/check-out`
- `POST /api/bookings/{year}/{id}/no-show`
- `POST /api/bookings/{year}/{id}/payment` (or route into booking-cost/payment aggregate)

## API-First Rollout Steps

1. Move all lifecycle predicates to API helpers in one place. ✅
2. Expose lifecycle list endpoint(s) for reservations, arrivals, stays, departures. ✅
3. Point landing widgets to those endpoints only. ✅
4. Point `/bookings?view=...` left column to same endpoint(s). ✅
5. Add action endpoints that mutate required fields safely. ✅ check-in, check-out, and no-show. Payment goes through the ledger transaction endpoint.
6. Add API integration tests for each state boundary and transition. ◑ state lists and the three actions are covered. The import classification cases at the bottom of this file are not.
7. Add client integration tests to verify each view consumes correct endpoint. ◑ view routing is covered. Button flows for check-in, check-out, and no-show are not.

## Import Rule

On import, do not trust or persist an independent lifecycle state enum.

Instead:
- normalize imported date/status fields,
- store booking fields,
- derive classification via the same API predicates used for live data.

Checkout-specific import rule:
- If legacy source provides an explicit checkout date/time, map it to `ActualDepartureDate`.
- If legacy source does not provide it, do not fabricate checkout; keep it null and classify with fallback rules.
- Never infer checkout solely from planned `DepartureDate`.

This guarantees imported and manually entered bookings classify identically.

## Still open

- Import classification tests listed below.
- A client test that performs check-in, check-out, and no-show from the booking page.
- Optional audit fields `CheckedOutAt` and `CheckedOutByUserId`.
- The single `GET /api/bookings/lifecycle?state=` query. The four list endpoints are the contract in use and are equivalent.

Planned import verification tests:
- Imported booking with `ActualArrivalDate` + `ActualGuestCount` + future `DepartureDate` appears in stays.
- Imported booking with no `ActualArrivalDate` and arrival within look-ahead appears in arrivals only.
- Imported booking departing today with valid arrival appears in departures.
- Imported cancelled/void/deleted booking is excluded from reservation/arrival/stay/departure lists.
