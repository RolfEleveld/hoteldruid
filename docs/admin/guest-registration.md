# Official guest files

This is the durable shape of the old Italian guest schedule. The hotel records the fields a jurisdiction requires, keeps or sends that record the way that jurisdiction requires, and shows on the stay whether it was sent. It does not copy the Italian screen, and it does not keep a scan of the identity document unless a template says the law requires the image.

The rules below are the public duties as they stood in November 2026. A canton, a city, or a new decree can change the detail. The template is where that change belongs.

## What is common

Every duty found here is the same act. The clerk sees the identity document and writes down a short list. The guest signs when the rule still demands a signature. The hotel then either holds the record or transmits the fields. A photograph or a scan of the passport is not part of that common duty. Spain's data-protection authority and the Dutch authority say a copy is too much. Switzerland, Italy, Germany, France, and Mexico City ask for the document to be shown and for fields to be written down.

The fields that recur:

- Surname and given names
- Date of birth, and place of birth where the country asks
- Nationality or citizenship
- Habitual address
- Document type, number, and place of issue
- Arrival, planned departure, and the number of nights
- Who accompanies a child, or who leads a group
- A signature and its date, only when the template requires one
- Whether the file is missing, ready, signed, held, or sent, and the receipt when a portal returns one

Statistics returns, such as the Italian ISTAT movement file and the Swiss HESTA or PASTA file, are a separate template. They count nights. They do not carry the identity document.

## By place

| Place | Who | What happens to the file | Signature | Image of the document |
| --- | --- | --- | --- | --- |
| Switzerland, foreign guests | Anyone accommodated for payment who is not Swiss | The Meldeschein goes to the canton. Federal law still requires the guest to sign a paper form. A 2026 proposal would allow an electronic form without that signature. It is not in force. | Yes, on paper, until that change | No. The guest shows the document. The clerk checks the form against it. |
| Switzerland, Swiss guests | Set by the canton | Often a local register or a visitor-tax system, produced to the police on request. Basel-Stadt wants the day's forms available to the police. St. Gallen keeps signed foreign forms for five years and keeps Swiss guests in its electronic system. | Depends on the canton | No |
| Italy | Everyone who stays, including Italians | Alloggiati Web, to the state police, within 24 hours of arrival, or at once if the stay is shorter. A family or a guided group is filed by the leader. Companions need less detail. | No | No. Type, number, and place of issue are typed in. |
| France | Guests who are not French, including other EU citizens | The fiche is signed on arrival, kept for six months, and given to the police or gendarmerie only if they ask. A child under 15 can be written on the adult's fiche. | Yes | No. The official fields do not include the document number. |
| Germany | Guests who are not resident in Germany | The Meldeschein is filled from the passport, kept, and available to the authority. It is not a nightly national upload. | The statute lists the fields and a check against the document. It does not require a copy. | No |
| Spain | Guests using the lodging | SES.Hospedajes, electronic, before or at the start of the stay | No | No. The data-protection authority forbids asking for a copy of the identity card or passport. |
| Netherlands | The person who booked | A night register: name, place of residence, document type, arrival, departure. Companions and children are outside that duty. | No | No. A copy, scan, or photo is not allowed. |
| Mexico City | Each guest | Show a current official photo identity. Record the full name, address, and time of entry. | No | The city rule does not require the hotel to keep the image. |
| Mexico, federal | No single national hotel filing of the Italian kind | The traveler's immigration form is a matter for the immigration authority at entry. State and city guest books differ. A template can add a field such as CURP where that place actually requires it. | | |

Germany's fields are arrival and planned departure, surname, given names, date of birth, nationalities, address, the number of foreign companions and their nationality, and the serial number of the passport. Italy adds sex, place of birth, and place of issue, and does not ask for the home address. France adds the mobile number and the email address, and does not ask for the document number.

## What is kept

The signed card is paper. A room with four guests prints four cards. Each guest fills one, the clerk checks it against the identity document, and the card goes into the paper file. The hotel does not store the card and does not store a scan. A scan, when it is used, is only a way to read the number, the name, and the place of birth onto the guest. Those values stay on the guest record.

The guest record already keeps the card's fields: surname, given name, gender, date of birth, birth city, birth region, birth nation, nationality, the address, phone, email, document type, document number, expiry, and the place the document was issued. It also keeps the Italian codice fiscale (`TaxCode`) and the partita IVA (`VatNumber`). The police schedule does not ask for the partita IVA. The invoice does, so it stays on the guest and a report includes it only when that report is ticked to send it.

The booking already keeps the stay: room, arrival, planned departure, actual arrival, actual departure, and how many guests arrived.

Two things are not on the booking, and they are what has to be added.

Each other person in the room is a guest record attached to that booking. Today the booking names one client, and Booking Guests is a separate page that asks for a client id and a guest number. The booking has to list every occupant, because a digital report is one line per person, not one line per room. A child or a group member can be marked as covered by the lead guest where the standard allows that.

The paper card for that person on that stay needs a state: printed, checked against the document, filed. That state is not a copy of the card. A digital report, when the hotel has one, needs its own state on the stay: included, sent, and the receipt. `DocumentsSent` on the guest is a free-text note and is the wrong place, because the same person stays more than once.

The booking form, when a guest is entered there, does not yet show the fields the card needs. They exist on the guest page and are absent from the booking form: gender, date of birth, birth place, nationality, the street address, document expiry, and the place of issue. Nation and city on the booking form are the address, not citizenship and not place of birth.

## How a hotel chooses the report

Setup chooses a registration standard the way it chooses a chart of accounts. The standard ticks three lists: fields of the guest, fields of each attached guest, and fields of the booking. It also ticks who is included and how the hotel delivers it: print a card and file the paper, produce a report a person can submit, or both.

The same choice sets how many named guests a room must have. One standard keeps a single guest on the room. Another requires one named guest for each person in scope. Until the guests arrive, that number is Expected guests. Once they have arrived, it is Guests on arrival. A room with two arriving guests then needs two guest records on that booking, and each of those records must have the ticked fields filled. A child or a group member covered by the lead guest counts as a name and does not need the full card, where the standard allows that.

A short count, or an empty required field on one of those guests, is a line on the night audit for that day. The night audit shows it. Signing the night means the desk has seen the line. When the standard produces a digital file for the authorities, the same gap is a line on that report, and the stay is not ready to send.

Custom has no default for this. Choosing custom means ticking the count as well as the fields:

- One named guest for the room, or one named guest for each person in scope
- Which people are in scope: every occupant, foreigners only, or the booker only
- Which fields the lead guest must have, and which fields each attached guest must have
- That a short count or an empty required field shows on the night audit
- That the same gap shows on the authority report, when custom is set to produce one

| Standard | Who | Guest fields ticked | Booking fields ticked | Delivery |
| --- | --- | --- | --- | --- |
| Switzerland, foreign guests | Guests who are not Swiss | Name, date of birth, document type and number | Arrival, departure | Print, sign, file the paper, and hand the form to the canton |
| Switzerland, local | Whoever the canton includes | Name, date of birth, and the canton's extra boxes | Arrival, departure | Print or the local register. Custom covers the canton. |
| Italy, Alloggiati | Every occupant. A family or a guided group may be filed under the lead guest. | Name, gender, date and place of birth, citizenship, document type, number, and place of issue. Partita IVA and codice fiscale stay off this report. | Arrival, nights | A report, one row per person, for the police portal |
| France | Guests who are not French. A child under 15 may sit on the adult's card. | Name, date and place of birth, nationality, address, phone, email. Document number stays off. | Arrival, planned departure | Print, sign, file for six months |
| Germany | Guests who do not live in Germany | Name, date of birth, nationalities, address, document serial, companions and their nationality | Arrival, planned departure | Print and keep |
| Spain | Each occupant | The SES boxes, which are the common identity fields | Arrival, departure | A report for SES.Hospedajes |
| Netherlands | The booker only | Name, place of residence, document type | Arrival, departure | Keep the night register |
| Mexico City | Each occupant | Name, address | Time of entry | Keep the register. No image. |
| Custom | The count the hotel ticks: one name for the room, or one name per person in scope | Whichever guest boxes are ticked, required on each name the count asks for | Whichever booking boxes are ticked | Print, report, or both. A gap shows on the night audit, and on the authority report when that report is on. |

Time of entry is the one value the booking does not store. Mexico City is the standard that needs it. A second nationality is the one value the guest does not store. Germany is the standard that can need it, and custom can add that box.
