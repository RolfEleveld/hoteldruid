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

## Still open
- [ ] Keep the per-booking transaction folder as a projection of ledger lines, not a second book.
  - Blazor side: Remove dual-write, delete `BookingTransactionRepository`, make transaction list a read-only projection of the ledger (using booking date range + `bookingId` filter). Implement immediately.
  - PHP export/import compatibility: Add checklist item for later update of the main (PHP) branch after Blazor work is complete and visually tested.
- [ ] Add a booking index so history is not a scan of every day partition.
- [ ] Bring PHP cash-register permissions, person, payment method, currency, and running cash total onto petty-cash posts.
- [ ] Stop deleting money history. Corrections should be reversing ledger entries.
- [ ] Bring over the PHP point of sale (`punto_vendita.php`) for walk-up sales and non-staying guest payments.
- [ ] Post inventory goods sold at the point of sale. PHP has no separate purchasing module.
- [ ] Post extra costs attached to a booking (`costiprenota`) when they are sold or cashed.
- [ ] Personnel is only `persona_costo` on a cash row in PHP. Bring that field over. Do not invent payroll.
- [ ] Invoice and contract totals must use ledger charges, prepayments, deposits, and balance.

## Nice to have (future visualization)

- Per-room timeline / production graph showing revenue earned per night, maintenance/renovation events, and issue-driven downtime as separate dimensions so room-level profitability and utilization can be analyzed over time. Pin for later after core ledger parity.

## Nice to have (documentation & UX validation)

- Living user manual in Markdown (`docs/user-guide/`) with one page per major screen/component, role-based index, and embedded Playwright screenshots that are regenerated on every release so the manual stays visually accurate and reviewable by desk clerks, accountants, and managers.
  - Collect usage notes, click paths, and scenario coverage per screen close to the source so changes can be reviewed together.
  - Include a visual-test pipeline (Playwright) that produces up-to-date screenshots per version.
  - Provide an index that lets a person in a specific role quickly navigate to the relevant screen and validate they are on the right page with the right information.
  - The approach (folder layout, screenshot embedding strategy, review workflow, and how tightly the manual is coupled to the code) still needs discussion and may be revised over time.
