# Ledger parity: PHP main to Blazor

The booking reference is a dimension, not a GAAP account. Hotel-level movements leave it empty. Tick items in this file as they land on the `blazor` branch.

## Done

- [x] Extend the global ledger line with journal id, account code, account group, debit, credit, cash register, payer, and source document.
- [x] Post a balanced pair for booking payments, deposits, and refunds.
- [x] Post expenses and money-history rows into the same ledger. Empty booking reference means hotel cash.
- [x] Query ledger lines by booking reference or by unassigned hotel cash.
- [x] Show booking ledger lines on the booking finance page and add a general ledger page.
- [x] Add a small USALI chart of accounts page. Current codes are constants: 1000 cash, 1200 guest ledger, 2400 guest deposits, 4100 room revenue, 4900 other income, 6100 operating expense.
- [x] API tests for a balanced journal, booking filter, deposit not reducing revenue, petty cash with an empty booking reference, and a non-staying payment.
- [x] Post the stay charge and extras when a booking is created, so cost exists from the moment of booking.
- [x] Post prepaid amount changes as ledger lines, not only as a booking field.
- [x] Recognize deposits as a liability until earned or settled. Do not reduce room revenue when the deposit is taken.
- [x] Post final settlement as the closing journal for the booking (data model + settlement fields + prepaid method/reference fields ready; UI/closing journal + focus navigation from dashboard done).
- [x] Keep the per-booking transaction list as a projection of ledger lines, not a second book. `BookingTransactionRepository` is removed. The list, finance totals, and documents collapse each booking-transaction journal to one row (sequence, applied-to, and created-by).
- [x] Add a booking index so history is not a scan of every day partition. The first read of a booking records its ledger line ids in `bookings/{bookingId}/_ledger_index.json`. Later reads open those sequence files directly. A journal posted after that index exists is appended to it.
- [x] Stop deleting money history. A correction posts the opposite journal and a new history row. The original row and its ledger lines stay.
- [x] A delete of a booking or expense that the ledger cites hides the record and keeps it for the audit trail. A record with no ledger lines is removed. `POST /api/retention/clear-deleted` removes a hidden record only when its year is before the current year and no ledger line still cites it.

- [x] PHP export/import compatibility. The PHP package keeps one booking money history, `payments{year}`. Blazor imports those rows as ledger journals and does not open a second per-booking transaction book. Extra costs stay on the booking.
- [x] PHP cash-register permissions, person, payment method, currency, and running cash total come across on the petty-cash posts. A booking payment's cash-register copy keeps those fields and does not post a second journal.
- [x] Personnel stays the person named on a cash row. That person is stored on the petty-cash post. There is no payroll.
- [x] Bring over the PHP point of sale (`punto_vendita.php`) for walk-up sales and non-staying guest payments. One page sells onto a checked-in stay or takes cash with an empty booking reference.
- [x] Post inventory goods sold at the point of sale. An article in a warehouse drops its quantity when the sale is written to the ledger, and a reversal puts that quantity back. A room article is an availability check. There is no cost-of-goods journal until a purchase cost exists.
- [x] Post extra costs attached to a booking (`costiprenota`) when they are sold or cashed. A sold extra debits the guest ledger and credits other income, and removal reverses that pair.
- [x] Invoice and contract totals must use ledger charges, prepayments, deposits, and balance. Checkout applies a held deposit with debit 2400 and credit 1200.

## Nice to have (future visualization)

- Per-room timeline / production graph showing revenue earned per night, maintenance/renovation events, and issue-driven downtime as separate dimensions so room-level profitability and utilization can be analyzed over time. Pin for later after core ledger parity.

## Nice to have (documentation & UX validation)

- Living user manual in Markdown (`docs/user-guide/`) with one page per major screen/component, role-based index, and embedded Playwright screenshots that are regenerated on every release so the manual stays visually accurate and reviewable by desk clerks, accountants, and managers.
  - Collect usage notes, click paths, and scenario coverage per screen close to the source so changes can be reviewed together.
  - Include a visual-test pipeline (Playwright) that produces up-to-date screenshots per version.
  - Provide an index that lets a person in a specific role quickly navigate to the relevant screen and validate they are on the right page with the right information.
  - The approach (folder layout, screenshot embedding strategy, review workflow, and how tightly the manual is coupled to the code) still needs discussion and may be revised over time.
