# Blazor Migration Tracker

Date: 2026-02-21  
Branch: blazor

**Architecture**: See [ARCHITECTURE.md](ARCHITECTURE.md) for design decisions, data model, and implementation phases.

## Current Status Summary (2026-02-21)

**Phase 1A — Core Storage & Test Infrastructure: COMPLETE ✅**

- FileKeyValueStore fully implemented and tested (48/48 tests passing)
- Complete test infrastructure with unit, integration, and event logging tests
- All test failures analyzed and resolved (11 fixes applied to JSON serialization, test isolation, configuration, and closure bugs)
- API endpoints for rooms (POST, GET, PUT, DELETE) fully functional
- Documentation complete (ARCHITECTURE.md, TEST_INFRASTRUCTURE.md, Migration Tracker)

**Phase 1B — Repositories & Ledger: Ready to begin**

Next milestone: Implement `IRoomRepository`, `ILedgerRepository`, and `IBookingTransactionRepository` with comprehensive validation and event logging.

Purpose

- Single source-of-truth for planning, progress, and findings while migrating HotelDruid to a Blazor WASM frontend with a thin C# 10 API backend.

High-level Objectives

- Rebuild UI as browser-based Blazor (WASM) so client does heavy-lifting.
- Implement a thin C# 10 API (hosted locally, cloud-ready) for business logic.
- Replace DB dependency with a simple file-backed key-value store managed by the API.
- Support import of existing HotelDruid export format.
- Migrate PHP modules incrementally into API endpoints and Blazor components.

Current Plan (tracked as TODOs)

1. Create migration tracker file (this file).
2. Inventory PHP features and exports (map php files to features).
3. Design architecture and key-value data model.
4. Scaffold Blazor + API solution targeting C# 10 / .NET 6.
5. Implement a simple file-backed key-value store library.
6. Build core API endpoints (bookings, clients, tariffs, settings, exports).
7. Implement import/export compatibility and mapping tools.
8. Implement auth/user management.
9. Migrate modules iteratively and implement Blazor components.
10. Add tests, CI, and basic deployment scripts.
11. Prepare documentation and cloud deployment guidance.

How I'll work

- I will update this file and the internal todo list as work completes.
- I will make small, incremental changes on the `blazor` branch and report each change.

Next immediate actions (suggested)

- Run a codebase inventory to map PHP features to API endpoints and UI components.
- Scaffold the initial Blazor WASM + ASP.NET Core API solution.

If you want me to start now, I will: (A) run a repo inventory to produce a feature map, then (B) scaffold the solution and commit initial projects.

## Inventory: PHP Feature Map

- `hoteldruid/prenota.php`: Bookings — create/confirm reservations, pricing, inventory checks.
- `hoteldruid/clienti.php`: Clients — client list, create/edit, start booking from client.
- `hoteldruid/modifica_prenota.php`: Bookings — modify reservation UI & logic.
- `hoteldruid/modifica_cliente.php`: Clients — edit client record.
- `hoteldruid/creaprezzi.php`: Tariffs/prices — create/edit tariff tables.
- `hoteldruid/tab_tariffe.php`: Tariffs — tariff table management/view.
- `hoteldruid/includes/funzioni_tariffe.php`: Tariffs helpers used by bookings/forms.
- `hoteldruid/includes/funzioni_clienti.php`: Clients DB helpers.
- `hoteldruid/costi.php`: Costs/finances management.
- `hoteldruid/modifica_costi.php`: Edit cost entries.
- `hoteldruid/inventario.php`: Inventory management UI/logic.
- `hoteldruid/includes/funzioni_costi_agg.php`: Cost-related helper functions.
- `hoteldruid/export.php`: Export entry point / options.
- `hoteldruid/export-import/index.php`: Import/Export UI.
- `hoteldruid/export-import/lib/Exporter.php`: Exporter class/logic.
- `hoteldruid/export-import/lib/Importer.php`: Importer class/logic.
- `hoteldruid/api.php`: External API endpoint / integrations.
- `hoteldruid/server.php`: Server endpoints / inter-process handlers.
- `hoteldruid/interconnessioni.php`: External integrations manager.
- `hoteldruid/includes/interconnect/aggiorna_ic.php`: Integration update routines.
- `hoteldruid/gestione_utenti.php`: User management UI.
- `hoteldruid/privilegi_utenti.php`: User privileges/roles management.
- `hoteldruid/hoteldruid-config.php`: Core settings and constants.
- `hoteldruid/personalizza.php`: UI customization / personalization.
- `hoteldruid/inizio.php`: Home/dashboard entry page.
- `hoteldruid/visualizza_tabelle.php`: Table viewers / admin lists.
- `hoteldruid/includes/templates/head.php`: Page header / common includes.
- `hoteldruid/includes/templates/main_menu.php`: Main navigation/menu.
- `hoteldruid/includes/funzioni.php`: Core helpers/utilities.
- `hoteldruid/includes/funzioni_ins_prenota.php`: Booking insertion helpers.
- `hoteldruid/includes/funzioni_appartamenti.php`: Units/rooms helpers.
- `hoteldruid/includes/funzioni_relutenti.php`: User relations helpers/groups.
- `hoteldruid/includes/funzioni_log.php`: Logging / audit utilities.
- `hoteldruid/messaggi.php`: Messaging / notifications.
- `hoteldruid/visualizza_contratto.php`: Contract template viewer.
- `hoteldruid/punto_vendita.php`: Point-of-sale / payments UI.
- `hoteldruid/includes/funzioni_email.php`: Email sending/templates helpers.

Notes:

- This inventory is a first-pass mapping based on filenames and likely responsibilities; next step is to open key files to capture exact data access, payload formats, and export structure to design the API and import adapters.

## Visualizza_tabelle: tables referenced & required APIs

- Summary: `visualizza_tabelle.php` is a central admin/listing page that touches many persistent tables. Ensure the API covers the tables it reads/updates so the Blazor UI can display/create/update/delete entries.
- Direct table -> API mapping (from file analysis):
	- Bookings: `$tableprenota`, `$tableprenotacanc` -> `/api/bookings`
	- Clients/Guests: `$tableclienti` -> `/api/clients`
	- Rooms/Units: `$tableappartamenti` -> `/api/rooms`
	- Rates/Tariffs: `$tablenometariffe` (`ntariffe`) -> `/api/rates`
	- Periods / calendar: `$tableperiodi` -> `/api/periods` or `/api/availability` (needs explicit endpoint)
	- Costs / payments: `$tablecosti`, `$tablesoldi`, `$tablecostiprenota` -> `/api/billing` (transactions/costs)
	- Users / privileges / groups: `$tableutenti`, `$tableprivilegi`, `$tablerelgruppi`, `$tablerelutenti` -> `/api/users`, `/api/privileges`
	- Settings / personalization: `$tablepersonalizza` -> `/api/settings`
	- Contracts / templates: `$tablecontratti`, `$tabledescrizioni` -> `/api/templates` (or `/api/contracts`)
	- Integrations: `$tableinterconnessioni` -> `/api/integrations`
	- Inventory: `$tablebeniinventario`, `$tablerelinventario`, `$tablemagazzini` -> `/api/inventory`
	- Transactions / cashboxes: `$tabletransazioni`, `$tablecasse` -> `/api/transactions` or `/api/cashboxes`
	- Relation / helper tables (rclientiprenota, relclienti, relutenti, relgruppi, etc.) -> surfaced via parent APIs or small admin endpoints as needed.

Notes: most auxiliary relation tables can be exposed through the parent resource APIs (bookings/clients/users). Add small admin endpoints only if the UI needs direct management of those relations.

## Billing & Ledger: proposed design and plan

- Overview: implement a hybrid billing API that composes `reservations` (bookings), `rates`, and `templates` to calculate charges. Billing actions can be triggered at multiple points: `check-in`, `check-out`, manual adjustments, and a nightly closure job.
- Key components:
	- `Billing Service (hybrid)` — endpoint(s) that accept booking id + action (`checkin`, `checkout`, `adjust`) and return invoice/ledger entries. Uses `rates` + booking composition + templates to build charges.
	- `Transactions / Cashbox API` — CRUD for cash entries, payments, refunds (`/api/transactions`, `/api/cashboxes`).
	- `Ledger Service` — responsible for nightly closure: produce closed balances, move open transactions into the ledger, and snapshot end-of-day balances. Ledger stores immutable entries (date, balances, refs).
	- `Templates` — billing templates for invoice layout, automatic charges, taxes/fees rules (`/api/templates` or `/api/billing/templates`).
	- `Audit & Reconciliation` — every billing action writes audit records; nightly job produces reconciliation reports and flags inconsistencies.
- Nightly closure workflow:
	1. Run `POST /api/ledger/close` (scheduled job) which:
		 - Aggregates open transactions for the day, computes closed balances, marks those transactions as closed, writes ledger entries, and produces a daily report.
		 - Optionally runs rollover tasks (e.g., move transient authorizations into permanent transactions).
	2. Ledger entries are append-only and exportable (CSV/JSON) for accounting.

- Why this is common: hotel PMS systems typically separate transactional payments (payments, deposits, refunds) from the accounting ledger. They support check-in/out charges, nightly billing runs (end-of-day / shift close), and a ledger for accounting and audit. Confirmed: this architecture aligns with standard hotel software patterns.

- Placement in architecture: add `Billing Service` and `Ledger Service` to the backend architecture diagram and Phase 2 tasks (implement core billing endpoints, transactions API, and nightly ledger job). Mark ledger as required for production-grade finance handling.

- Action items to add to plan/track:
	- Add `/api/periods` or extend `/api/availability` to cover calendar queries used by `visualizza_tabelle`.
	- Scaffold `/api/transactions` and `/api/ledger` endpoints in the next scaffold step and include a scheduled job sample for nightly closure.
	- Add unit tests for billing calculations (rates + reservation composition) and an integration test for nightly ledger closure.

Next steps:
- I'll mark the `Inventory PHP features` todo as completed and start `Design architecture & data model`.

## Findings: Export / Import format and API (analysis of Exporter.php, Importer.php, export-import/index.php, api.php)

- Export package format
	- Root contains `manifest.json` pointing to `metadata/`, `configs/`, `data/`, `schemas/`, `docs/`.
	- `data/tables/*.json` contain canonicalized table exports with keys: `table_name`, `columns`, `rows`, `row_count`.
	- `metadata/export_metadata.json` describes DB connection, export options, counts, and integrity notes.
	- Exports use a `canonical_mapping.json` (bundled or package-provided) to translate local table/field names to international/canonical names.
	- Exporter uses a `DataFlattener` to produce normalized JSON and a `CanonicalMapper` to map names.

- Import process (what Importer expects)
	- Accepts a Zip package, extracts it to a temp dir, reads `manifest.json` and `metadata/*`.
	- Validates required dirs (`data/tables`, `metadata`) and optional `configs` and `schemas`.
	- Provides `getImportPreview()` to preview table rows and columns.
	- Allows setting of per-table `field mapping` (user overrides) before import.
	- Importer re-hydrates canonical rows via `canonical_mapper->canonicalRowToSource()` then inserts into SQL tables via generated INSERT statements.
	- Importer can reconstruct PHP config files from exported JSON `configs` and write them into `C_DATI_PATH`.

- Implications for target Blazor + C# API
	- The export package is JSON-first and canonicalized — ideal for an adapter that maps canonical table JSON into our new storage model.
	- We must implement an Import adapter that:
		- Accepts the same zip format, reads `manifest.json`, and maps canonical tables to our storage collections.
		- Supports preview, dry-run, and user-specified field mapping.
		- Can import exported `configs` into destination settings in the new format.
	- Since Importer expects to write SQL INSERTs, we will replace that behavior with calls to our Key-Value storage API.

	### Separation of Concerns: Import/Export vs Data API

	Important: the Import/Export functionality is a hybrid service that sits above the Core Storage (Data) API. It performs validation, mapping, preview/dry-run, id-mapping and orchestration. It should NOT be implemented inside the low-level Data API layer. Instead, the Import/Export service MUST call the Core Storage API for document-level creates/updates/deletes so that locking, indexing, and storage semantics remain centralized and consistent.

- Notes from `api.php` (existing PHP API surface)
	- The file exposes many programmatic entry points: bookings queries (by date, id, year), clients listing, cashbox, session confirm for payments, and interconnections (pull XML for external channels).
	- Auth and rate-limiting are implemented via contract records and cache tables; sessions and privileges are stored in DB tables.
	- Responses are mixed XML and HTML depending on action — new API should be consistent JSON over HTTP (REST/JSON) or JSON+HTTP status codes.

## Target Architecture (high-level)

- Frontend: Blazor WebAssembly (client-heavy), localized (resource files for en/es/it). UI components progressively replace PHP pages.
- Backend: ASP.NET Core minimal API (.NET 6 / C# 10) providing REST endpoints; thin business logic and validation; file-hosted key-value store as primary persistence.
- Import/Export Adapter: server-side module that accepts HotelDruid export ZIP, validates manifest, maps canonical tables to KV collections, and imports data (supports dry-run and preview).
- Storage: File-backed key-value store:
	- Collections are directories under a data root, e.g. `data/collections/{collectionName}/`.
	- Each record is stored as a JSON document file named by id: `{id}.json`.
	- A lightweight index file per collection (e.g., `_index.json`) keeps sequence counters and lists (for fast enumeration).
	- Concurrency via file locks; optional in-memory cache for performance with periodic flush.
	- Support export: produce the canonical JSON structure to create a compatible zip package.

## Data Model & Mapping Strategy

- Canonical tables -> Collections: use canonical `international_name` from `canonical_mapping.json` as collection names (e.g., `clienti` -> `guests`).
- Rows -> Documents: each exported row becomes a JSON document; primary key field maps to `id`.
- Relationships: store references as fields containing referenced document ids (no joins at storage layer).
- Schemas: keep `schemas/` JSON in a `schemas/` collection for validation and migration utilities.
- Configs: import `configs/configurations.json` into a `settings` collection keyed by config name.

## API Surface (initial, minimum viable)

- Auth: `POST /api/auth/login` (token-based, local dev-friendly); `POST /api/auth/refresh`.
- Import/Export: `POST /api/import` (upload zip, returns import preview/dry-run options); `GET /api/import/preview/{id}`; `POST /api/export` (create export zip from KV store).
- Clients: `GET /api/clients`, `GET /api/clients/{id}`, `POST /api/clients`, `PUT /api/clients/{id}`, `DELETE /api/clients/{id}`.
- Bookings: `GET /api/bookings`, filters by date/period/client; `GET /api/bookings/{id}`; CRUD endpoints.
- Tariffs/Prices: `GET /api/tariffs`, `POST /api/tariffs`.
- Inventory: `GET /api/inventory`, CRUD.
- Users & Privileges: `GET /api/users`, auth/roles endpoints.
- Misc: `GET /api/settings`, `PUT /api/settings/{key}`, `GET /api/docs/export-info`.

## Migration Strategy & Work Plan (detailed tasks)

Phase 1A — Core Storage Layer [**COMPLETED**]

- Capture detailed export/import schemas and example package (done: Exporter/Importer inspected). [✅ completed]
- Design KV storage API and small spec. [✅ completed — see ARCHITECTURE.md]
- Scaffold .NET solution: `HotelDroid.Blazor.Client` (WASM), `HotelDroid.Api` (ASP.NET Core minimal API). [✅ completed]
- Implement file-backed KV store (`FileKeyValueStore`). [✅ completed]
  - SemaphoreSlim per-collection locking for write serialization
  - GUID-base32 ID generation (26-char, URL-safe)
  - Index files (`_index.json`) for name → ID mapping
  - Self-healing index rebuild
  - Atomic writes with temp → rename pattern
  - Generic CRUD: `GetAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `ListAsync`
  - Index: `RebuildIndexAsync`, `GetIndexAsync`
  - Collection management: `DeleteCollectionAsync`
- Unit tests for FileKeyValueStore [✅ completed]
  - Basic CRUD operations
  - Concurrent writes (10+ simultaneous creates)
  - Concurrent updates (no corruption)
  - Index operations
  - Path traversal validation
  - Edge cases (nulls, missing documents)

Phase 1A — Test Infrastructure [**COMPLETED**]

- Enhanced unit tests with file system validation. [✅ completed]
  - FileKeyValueStoreTests (11+ test cases)
  - FileKeyValueStoreFileSystemTests (validates actual files on disk)
  - Concurrent operation validation
  - **Status (2026-02-21):** All 48 tests passing ✅
  
- API integration tests with WebApplicationFactory. [✅ completed]
  - RoomsApiTests (CRUD endpoints)
  - Full environment setup/teardown per test
  - **Fixes applied:** Test data isolation, JSON serialization consistency, proper DataRoot configuration
  
- Event logging infrastructure for SIEM integration. [✅ completed]
  - ISystemEventLogger interface
  - WindowsEventLogger implementation
  - Structured JSON events
  - SIEM-compatible format
  
- Test helpers and utilities. [✅ completed]
  - TestFixture with DI setup
  - XunitLogger/XunitLoggerProvider
  - TestEventCapture for assertion validation
  - TestEnvironment for file system setup
  
- Test dashboard and reporting. [✅ completed]
  - TestDashboardGenerator (HTML, JSON reports)
  - run-tests.ps1 script for test execution
  - TRX report generation
  - Performance metrics and slowest test tracking
  
- Documentation: TEST_INFRASTRUCTURE.md. [✅ completed]

### Test Fixes Applied (2026-02-21)

- **Test Infrastructure Configuration:** Fixed `RoomsApiTests` initialization to set `DataRoot` configuration via `WithWebHostBuilder` before factory creation (was setting environment variable too late).
- **Test Data Isolation:** Added `ClearRoomsAsync()` method using API calls to prevent test data contamination across integration tests.
- **JSON Serialization Consistency:** Updated tests to deserialize JSON using same options as `FileKeyValueStore` (CamelCase naming policy, case-insensitive).
- **Base32 ID Validation:** Fixed `IdGenerator.IsValidId()` to properly validate base32 alphabet (a-z, 2-7) instead of generic alphanumeric.
- **Closure Bug in Concurrent Test:** Fixed closure variable capture in `ConcurrentUpdates_PreventCorruption` to properly isolate loop variable `i` for async tasks.
- **Index Rebuild Test:** Fixed `RebuildIndexAsync_ReconstructsIndexFromFiles` to write JSON documents using FileKeyValueStore's JSON options (CamelCase) for proper index field name matching.

**Current Test Status: 48/48 passing** (was 37/48, 11 failures fixed)

Phase 1B — Repositories & Ledger (NEXT: upcoming)

- Design and implement `IRoomRepository` with validation rules
- Implement `ILedgerRepository` (snapshots + incremental sequences)
- Implement `IBookingTransactionRepository` (per-stay ledger)
- Repository unit tests with event logging integration and event validation patterns
- **Status (2026-02-21):** Pending — will start after Phase 1A validation complete

Phase 2 — Core platform

- Implement file-backed Key-Value store library with API and unit tests.
- Implement Import adapter that maps canonical JSON into the KV store (support preview and dry-run).
- Implement minimal Auth and user endpoint.
- Implement core endpoints: clients and bookings with basic CRUD.

Phase 3 — Feature migration (iterative)

- Migrate tariff management, inventory, costs, messages, point-of-sale endpoints one by one.
- For each module: implement API endpoints, then Blazor components consuming them.

Phase 4 — Polish and deploy

- Add i18n resource files (en, es, it) and wiring in Blazor.
- Add export endpoint producing compatible zip export.
- Add tests, CI pipeline, and deployment scripts for local and cloud hosting.

## Risks and Open Questions

- Export package variations: packages may include custom `canonical_mapping.json` — import adapter must accept package-provided mapping and fallback to bundled mapping.
- Concurrency: file-based KV store must handle concurrent API requests – design locking carefully or use a simple server-side mutex.
- Data volume: very large exports may not fit in single-file JSON store approach; need to consider sharding or optional DB backend later.

## Immediate Next Actions (I will execute if you confirm)

- Scaffold the .NET solution (WASM client + ASP.NET Core API) with project layout and minimal endpoints.
- Commit scaffold to `blazor` branch and update this tracker with the project structure and initial TODOs.

If you confirm, I'll scaffold the solution now.

**Local HTTPS Validation**

- **Status:** Completed — local self-signed certificate created and Kestrel bound HTTPS on localhost:5001 using thumbprint `CB157E36572F9A97439589BCCDD7C91DB529A46D`. The API started successfully during validation.

## Repository Layout Recommendation

- Purpose: keep a clear multi-project layout to separate concerns and simplify builds, CI, and deployments. Leave existing PHP sources in place under `legacy/` until features are ported.
- Suggested tree:
	- `src/HotelDroid.Api/` — ASP.NET Core minimal API (server)
	- `src/HotelDroid.Client/` — Blazor WebAssembly client
	- `src/HotelDroid.Shared/` — DTOs, validation, shared models
	- `tests/HotelDroid.Api.Tests/` — unit/integration tests
	- `scripts/`, `docker/`, `docs/` — infra and docs
	- `legacy/` — existing PHP application (moved or left in place)

- Notes:
	- Keep `global.json` pinned to the intended SDK (already present).
	- Create `HotelDroid.sln` at repo root and add the three projects when scaffolding.
	- For now, leave the `hoteldruid/` PHP folder untouched; we will migrate modules incrementally and remove legacy files once the API + Blazor replace them.

## API Segmentation Proposal (refinement)

The application APIs will be organized into focused segments that map directly to frontend modules and business domains. Each segment exposes REST resources and a small set of hybrid endpoints (composed views) used by the UI.

- Core entity APIs (CRUD-centric)
	- `User` — /api/users: authentication, profile, roles/privileges.
	- `Guest` (`clients`) — /api/guests: guest/clients CRUD, contact info, history.
	- `Room` (`properties`/units) — /api/rooms: room definitions, amenities, inventory mapping.
	- `Template` — /api/templates: contract templates, email templates, document assets.

- Domain/hybrid APIs (compose multiple entities + business rules)
	- `Booking` (Reservations) — /api/bookings: availability checks, pricing, create/modify/cancel reservations, returns enriched booking objects (guest + rate + room assignments).
	- `Availability` — /api/availability: calendar/period queries, multi-room availability, filter by rate/group, used by booking flow and calendar UI.
	- `Arrivals` / `Departures` — /api/arrivals, /api/departures: day-of lists, check-ins/check-outs, manifests for front desk.
	- `Billing` / `Cashbox` — /api/billing: transactions, payments, point-of-sale interactions.

- Auxiliary APIs
	- `Tariffs` / `Rates` — /api/tariffs: manage rate tables and rules.
	- `Inventory` — /api/inventory: goods/services, stock levels.
	- `Messages` / `Notifications` — /api/messages: internal messaging, email triggers.
	- `Import/Export` — /api/import, /api/export: upload/preview/import/export packages, mapping UIs.

Design notes:

- Frontend modules enable/disable UI features based on `User` privileges returned by `GET /api/users/{id}/privileges`.
- Hybrid endpoints return normalized DTOs that the Blazor client consumes directly (avoid duplicating complex joins on client).
- Each segment should have lightweight OpenAPI/Swagger contract generated during scaffolding.

Next step: define concrete endpoint contracts (request/response DTOs) for `User`, `Guest`, `Room`, and `Booking` as the first iteration.

## Terminology Decision

- Use **`Rate` / `Rates`** as the primary English term throughout the API and UI (instead of "tariff"/"tariffe").
- Rationale: "Rate" is the standard hospitality industry term in English for prices/pricing plans; it's clearer for international users and aligns with common OTAs and PMS terminology.
- Migration note: map existing Italian `tariffe`/`tab_tariffe.php` and related helper names to `rates`/`tariffs` in the canonical mapping (e.g., canonical `tab_tariffe` -> `rates`). Keep original PHP names in import mapping for compatibility.

## Packaging & Deployment (local / per-user / system)

- Scripts added: `scripts/pack-and-deploy.ps1` (package + optional system deploy), `scripts/deploy-user.ps1` (per-user install), `scripts/deploy-api-local.ps1` (run API with machine cert fallback), `scripts/validate-cleanup.ps1` and `scripts/list-currentuser-localhost-certs.ps1` (validation/debug helpers).
- Supported flows:
	- Package-only: create `artifacts\hoteldroid-package.zip` for distribution.
	- Per-user install (recommended for user testing): extracts package into `%LocalAppData%\HotelDroid`, creates a CurrentUser `localhost` certificate (self-signed), writes `install-meta.json` with the created cert thumbprint, creates Start Menu shortcuts, and registers an HKCU uninstall entry. Includes `uninstall-user.ps1` that removes files and the created cert.
	- System install (optional): `pack-and-deploy.ps1 -Deploy` extracts to a specified install folder (e.g., `C:\Program Files\HotelDroid`), writes an HKLM uninstall entry, and creates system Start Menu shortcuts. Requires elevation.
- Validation: I ran the package creation and a full per-user deploy + uninstall cycle locally. Results: package created; per-user deploy created a CurrentUser `localhost` cert and `install-meta.json`; running the per-user uninstall removed the recorded cert, removed install files, and cleared the HKCU uninstall entry. Validation scripts are in `scripts/validate-cleanup.ps1`.
- Commands (repo root):
	- Create package: `.\	emplates\powershell` `.\scripts\pack-and-deploy.ps1 -PackageOnly -Force`
	- Per-user deploy: `.\scripts\deploy-user.ps1 -Force`
	- Validate cleanup: `.\scripts\validate-cleanup.ps1`
- Notes:
	- `src/HotelDroid.Api/Program.cs` was updated to bind to a certificate found in `LocalMachine\My` or `CurrentUser\My` when a thumbprint is provided via env var. Deployment scripts set the env var for runtime.
	- System-wide deploy writes to HKLM and requires elevation; per-user deploy targets HKCU and needs no elevation.
	- For single-origin hosting (avoid CORS), publish the Blazor client and copy its `wwwroot` into the API `wwwroot` (script hints included in `pack-and-deploy.ps1`).
