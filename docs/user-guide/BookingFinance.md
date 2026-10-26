# Booking Finance – End-to-End Flow (Desk Clerk)

This document describes the exact click path a desk clerk follows to create a booking, assign a tariff, record a payment, record a refund, and verify everything appears on the invoice and receipt.

## Prerequisites (one-time setup)

1. Open the application at `https://localhost:5001`
2. Go to **Rooms**
   - Create room **102** (or any available room) if it does not exist
3. Go to **Tariffs**
   - Create at least one tariff that applies to room 102 (e.g. "Standard 2025")
   - Make sure the tariff is linked to room 102

## Step-by-step scenario

### 1. Create a new booking
- Navigate to **Bookings** → click **New**
- Fill:
  - Client (search or create)
  - Room = **102**
  - Arrival Date, Departure Date
  - Status = Confirmed
- Click **Save**
- **Expected**: The booking appears in the list. TariffId and TariffTotalPrice must be populated (visible in the edit form or via API).

### 2. Record a payment
- Select the new booking → click the **Finance** link (or open Booking Finance page)
- In the top form:
  - Type = **Payment**
  - Amount = **150**
  - Applied to = **Stay balance**
  - Recorded by = your name
- Click **Record**
- **Expected**: The payment appears in the transaction list under the reservation row. Total Due is reduced.

### 3. Record a refund (overpayment scenario)
- Still on the same booking in Booking Finance
- In the top form:
  - Type = **Refund**
  - Amount = **4**
  - Applied to = **Stay balance**
  - Recorded by = your name
- Click **Record**
- **Expected**: The refund row appears in the list. Total Due is updated accordingly.

### 4. Verify on Invoice and Receipt
- Click **Invoice** (opens in new tab)
  - The payment (150) and the refund (–4) must appear as separate lines in the Payments section.
- Click **Receipt**
  - Same two lines must be visible.

### 5. Edge case – booking with zero due
- Create or select a booking that has already been fully paid (Total Due = 0.00 in the list)
- Click it
- **Expected**: The middle panel loads with the full transaction history (including the final payment that zeroed the balance). The form is not empty.

## Notes for automated tests (Playwright)

The Playwright test `BookingFinanceE2ETests.cs` should follow exactly the steps above, including:
- Room and tariff setup if they do not exist
- Full creation of a booking with tariff resolution
- Recording of payment + refund
- Assertion that both appear in the transaction list and on the generated Invoice/Receipt documents

All financial data must be read from the ledger (via the booking ledger index) – never from the removed `BookingTransactionRepository`.