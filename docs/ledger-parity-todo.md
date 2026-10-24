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

## Still open

- [ ] Post prepaid amount changes as ledger lines, not only as a booking field.
- [ ] Recognize deposits as a liability until earned or settled. Do not reduce room revenue when the deposit is taken.
- [ ] Post final settlement as the closing journal for the booking.
- [ ] Keep the per-booking transaction folder as a projection of ledger lines, not a second book.
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
