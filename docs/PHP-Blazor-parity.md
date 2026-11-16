# PHP capabilities still to cover

The daily desk is already in place: rooms, reservations, arrivals, stays, departures, clients, tariffs, extra costs, the calendar, cash, expenses, the ledger, inventory, and the documents the desk prints. This list is the surrounding behavior the PHP hotel had. Each item is handled in this system, not copied as a PHP screen.

This file is the record of what we decided. A point that is already in the desk says so. The others are decided and still to build.

## 1. A booking that arrives from outside

PHP could take a reservation from its own website form and from an interconnection. The desk can store an integration name and can list a received booking. Nothing yet accepts that booking into the reservation book.

An outside reservation arrives as an incoming message on the same batch service as point 2. The hotel does not call a channel and stores no password for one. An integration tool posts the structured message into the inbox. A person reviews booking requests on that list and accepts one into an ordinary booking: room, dates, guest, and tariff.

Automation is off unless deploy or an administrator turns it on. When it is on, the hotel runs four steps on a structured inbox message: detect what it is, assign the room and the tariff, process it into a booking, and write the confirmation. That confirmation is an outgoing message on the same service. The PHP website form and the PHP interconnection screen are not copied.

The integration points are a pub/sub API the hotel keeps. The topics are incoming received, booking request waiting for review, booking confirmed, outgoing ready, and outgoing relayed. An integrator reads them by cursor. The hotel does not push to a URL.

The tool calls those APIs with a Keycloak service account, not with a person's password. The realm template includes the client `HotelDruid-messages`. Its access token carries `hoteldruid_roles` of `messages`. That role may read ready messages, post an incoming message, write the relay time, read the cursor, and call structured inbox processing. Processing still runs only when deploy or an administrator has turned automation on. Otherwise the message stays for a person. The hotel does not create this client and does not store its secret. Keycloak issues the secret when the realm is imported, and the tool keeps it.

The desk list is on Messages. A structured inbox message becomes a booking when it carries a name, dates, and a room, from Accept or from Process when automation is on. Setup holds that switch.

In the desk. A portal adapter is still later.

## 2. Sending a document

A confirmation, an invoice, or a receipt is a row on a ready-to-send list, not a file the clerk downloads and not a message the hotel sends. The hotel stores no mailbox password, no WhatsApp token, and no SMS key. An integration tool queries the API for messages that are ready, relays them on the channel it owns, and writes back the moment they were relayed.

One message shape covers email, WhatsApp, SMS, post, and any later channel:

- Channel
- To and from
- Time the desk asked for it to be sent
- Delivery due
- Subject
- Body
- Attachments, when that channel needs them
- The booking, the invoice, or the other record it belongs to
- Ready, then relayed, with the relay time written back

Email is written inline. The body is the document. It is not a PDF attached to a short note. An attachment is how invoices get treated as suspicious mail, and the email channel leaves that field empty. The body carries a payment QR code, and a track-back QR code for the booking and for the invoice, so the reader can open the original. Documents can already draw a booking QR. The payment QR and the invoice QR are part of this message. WhatsApp, SMS, and post use the same row. Post may use the attachment field, because a letter is paper.

The desk shows the list. A clerk can open a row, see the to, the from, the due time, the subject, and the body, and leave it ready for the tool. The Email invoice button today downloads an `.eml` file. That button becomes "put this on the list." The row for email still has the `.eml` fields, so the tool can build the message without the hotel owning a mailbox.

The same `HotelDruid-messages` client from point 1 is how the tool queries the list and writes the relay time without a person signing in.

The Email invoice button puts the document on that list. The row keeps the `.eml` text, and the body carries the payment QR and the track-back QR codes. Email leaves the attachment empty.

In the desk. The hotel still does not send the message.

## 3. The official guest file

The Italian Alloggiati schedule and the ISTAT movement file are in the old hotel because the original team was Italian. They are two standards among others, not the model. The country-by-country fields are in [guest registration](admin/guest-registration.md). The duties there are the public rules as they stood in November 2026. A canton or a new decree changes the standard, not the desk.

The signed card is paper. A room prints one card per person. The guest fills it, the clerk checks it against the identity document, and the card goes into the paper file. The hotel does not store the card. A scan is only a way to read the name, the number, and the place of birth. The image is not kept. Those copied values stay on the guest record, which already has them: surname, given name, gender, date of birth, place of birth, nationality, address, phone, email, document type, number, expiry, and place of issue. The Italian codice fiscale and the partita IVA stay on the guest because the invoice needs them. A police report includes them only when that report is ticked to send them. Alloggiati is not ticked for either.

The booking already keeps the stay: room, arrival, planned departure, the actual dates, Expected guests, and Guests on arrival. A new guest typed on the booking can take gender, date of birth, place of birth, nationality, a second nationality, the street address, document expiry, and place of issue. Nation and city remain the address.

Every person the standard counts is a guest attached to that booking. Today the booking names one client, and Booking Guests is a separate page that asks for a client id. The booking has to list the names itself.

Setup chooses a registration standard the way it chooses a chart of accounts. The standards are Switzerland for foreign guests, Switzerland for the local canton, Italy Alloggiati, France, Germany, Spain, the Netherlands, Mexico City, and Custom. Each standard ticks three lists: fields of the guest, fields of each attached guest, and fields of the booking. It also ticks who is in scope and how the hotel delivers the file: print a card and file the paper, produce a report a person can submit, or both. Statistics returns, such as ISTAT and the Swiss HESTA or PASTA file, are a separate standard. They count nights and do not carry the identity document.

The same choice sets how many named guests a room must have.

- One standard, such as the Dutch night register, keeps a single name on the room.
- Another, such as Italy, Spain, and Mexico City, requires one named guest for each person in scope.
- Until arrival, the number is Expected guests. Once they have arrived, it is Guests on arrival. Two arriving guests means two guest records on that booking, and each record must hold the fields that standard ticks.
- A child or a group member covered by the lead guest counts as a name and does not need the full card, where the standard allows that.

A short count, or an empty required field, is a line on that day's night audit. Signing the night means the desk has seen the line, and the signed day keeps that list. When the standard produces a digital file, `GET /api/guest-files` carries the same gap and the day is not ready to send. Sending can stay a person filing the portal, then marking the file sent and storing the receipt. An adapter for a portal can come later.

Each person on that stay has a paper state: printed, checked, filed. A digital report has its own state on that stay: included, sent, and the receipt. `DocumentsSent` on the guest is a free-text note. The same person stays more than once, so the filing state does not live there.

Custom has no default. Choosing it means ticking all of the following:

- One named guest for the room, or one named guest for each person in scope
- Who is in scope: every occupant, foreigners only, or the booker only
- Which fields the lead guest must have, and which fields each attached guest must have
- Which booking fields go on the card or the report
- Print, a digital report, or both
- That a short count or an empty required field shows on the night audit
- That the same gap shows on the authority report, when custom is set to produce one

Time of entry is stored on the booking. Mexico City is the standard that needs it. A second nationality is stored on the guest. Germany is the standard that can need it. The built-in standards set the count, the scope, and the fields. Setup chooses which standard. Custom reads `RegistrationCustom` on the hotel configuration. Until that profile is saved, Custom keeps one name on the room and checks no extra fields.

In the desk. The other names on a room are still entered on Booking guests, not beside the booker on the booking screen.

## 4. A backup that runs itself

`HotelDruid.cmd Install -BackupFolder` schedules a nightly call to the export API and writes the zip to that folder. OneDrive or a network share is the intended destination. The task is named HotelDruid Backup. It runs as the person who installed the desk, so the folder is one that person can write while signed in. The clock time, the interval from 1 to 30 days, and how many days of zips to keep are arguments of that install. Running Backup again with a folder replaces the schedule. Running Backup with no folder takes one export and leaves the schedule as it is.

Logs are one file per day, `api-yyyyMMdd.log`. The nightly job deletes daily logs and backup zips older than the keep period. It leaves `api-startup.log` and `backup.log`. `api-startup.log` is deleted only while the desk is stopped.

The steps are in [backup and logs](admin/backup-and-logs.md).

Decided, and the installer does this. A desk that is already running keeps its old schedule until it is installed again with the folder.

## 5. A desk that knows who is allowed in

Privileges are stored. The four desk roles are `administrator` (the whole hotel, including setup and who may sign in), `clerk` (the desk, reservations, guests, arrivals, stays, cashier, and reports), `room-service` (room status and who is staying and leaving), and `reports` (reports only). A fifth role, `messages`, is the service account in point 1. It is not a person at the desk. The pages stay open until a proxy and Keycloak stand in front of the hotel. The hotel does not call the Keycloak admin API and does not create accounts. A person is given a role in the Keycloak console. The claim that reaches the hotel is `hoteldruid_roles`.

Decided. The roles exist. The pages stay open until that proxy is in front.

## 6. The working year, the rate period, and a room taken off sale

The booking year is the arrival year. The year boxes stay off the desk. The periods page stays off the desk. The periods API remains.

A rate is a tariff with dates. An extra cost has its own dates. There is no separate rate-period calendar to maintain.

A room that cannot be sold is a maintenance block or a block booking. That is the whole of the old "unsold room" case. No third kind of hold is added.

Decided, and this is how the desk works.

## 7. A PHP hotel brought across, including its invoices

Import already posts the room charge and the cash journal once. The closed bill can be shown again from those lines. The invoice total is not posted a second time. An extra cost that never reached the cash journal is the one new ledger line.

The rebuilt bill does not carry the invoice number that was printed in PHP. When the export has that number, import stamps it on the ledger lines of that booking, next to the booking number. Those lines are that invoice. The person moving the hotel stores any printed original. The screen shows the lines under the old number. A print made here is a new invoice. It does not reuse the old number and it is not a reprint of the original.

Guest-facing invoice numbers are one series per year. An imported invoice keeps the number PHP printed. A new invoice takes the next number in the new series. The store id stays a guid. The client's document number is the identity document, not the invoice.

Import stamps that number on the stay charge and on the payments of that booking. The ledger shows it under the booking. A print of an invoice or a Rechnung takes the next number in the year, printed as 2026-00042, and later prints of that stay reuse it. The imported number stays on the ledger lines.

In the desk.

## 8. Who may sign in, derived once

PHP privilege rows are not imported and are not written into this hotel. Each person is proposed as one of the four roles in point 5, from how wide that person's PHP allow-list was:

- Unrestricted, or a PHP administrator, becomes `administrator`
- Cash register or tariff becomes `clerk`
- Rooms, without cash, becomes `room-service`
- No permission to change the hotel becomes `reports`

When the years disagree, the latest year wins. The import's only output is that confirmation list. A person applies it in the Keycloak console. The hotel does not create the accounts. Nobody on that list is proposed as `messages`. That role belongs to the service client.

Import writes that list to `role_confirmations` and does not write privilege rows. `GET /api/role-confirmations` is the list a person applies in the Keycloak console.

In the desk.
