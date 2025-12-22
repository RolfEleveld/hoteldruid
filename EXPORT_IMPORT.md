# Export/Import

This is an overview for the export/import. It reflects what the code currently does and what is still missing.

## Current Implementation (code reality)

- **UI & entrypoint:** Admin-only UI is rendered from [hoteldruid/export-import/export-import-handlers.php](hoteldruid/export-import/export-import-handlers.php) + [hoteldruid/export-import/lib/ExportImportUI.php](hoteldruid/export-import/lib/ExportImportUI.php) when included by crea_backup. Exports land under `C_DATI_PATH/export-import/packages`. Preview mode uploads the ZIP to a temp file; mapping suggestions stay empty because no `metadata/entity_mapping.json` is written.
- **Export pipeline:** [hoteldruid/export-import/lib/Exporter.php](hoteldruid/export-import/lib/Exporter.php) uses [hoteldruid/export-import/lib/DataFlattener.php](hoteldruid/export-import/lib/DataFlattener.php) and the bundled [hoteldruid/export-import/canonical_mapping.json](hoteldruid/export-import/canonical_mapping.json) to write canonical table names. It produces `manifest.json`, `metadata/export_metadata.json`, `data/tables/*.json`, `schemas/tables/*.json`, and two doc text files. Config export only covers `lingua.php` and `tema.php`; `include_documents` is unused; templates copy only the fixed paths `../includes/templates/` and `../includes/hoteld_doc_backup.php` if present. No `metadata/entity_mapping.json` or `metadata/canonical_mapping.json` is added to the package, and most metadata fields are stubbed (no real duration, row totals, relationships, or validation results).
- **Import pipeline:** [hoteldruid/export-import/lib/Importer.php](hoteldruid/export-import/lib/Importer.php) unzips to a temp folder, checks for `manifest.json` and `data/tables`. It loads canonical mapping from the bundled JSON (not from the package) and inserts rows with `INSERT ... VALUES (?, ?, ...)` but never binds parameters—this likely fails against `esegui_query` and is not transactional. There is no schema/type validation, FK checking, rollback, or dry-run; mapping suggestions are empty without a packaged mapping file. Config import rewrites PHP files directly under `C_DATI_PATH` from JSON content.
- **Background worker:** [hoteldruid/export-import/trigger.php](hoteldruid/export-import/trigger.php) and [hoteldruid/export-import/bin/flatten-worker.php](hoteldruid/export-import/bin/flatten-worker.php) create `dati/export-runs/<run>/` with `state.json` and per-table JSON, then zip that run. This flow is not hooked into the UI, writes a different package shape (no metadata/entity mapping), and relies on a marker file to prevent concurrent runs.

## Documentation gaps vs reality

- Existing markdowns describe full validation, rollback, entity mapping in the package, comprehensive config/template/document export, and UI-backed background runs. The shipped code does not implement those pieces: inserts are unbound and non-transactional; `include_documents` is a no-op; template coverage is narrow; mapping files are not emitted; validation is minimal; background worker output is not exposed in the UI.
- Several documents still present pre-implementation design states; this file should be treated as the single accurate snapshot until the code is completed.

## TODOs (missing or partial)

1. **Package completeness:** Emit `metadata/entity_mapping.json` and the canonical mapping used at export time; add relationship/schema/version files or adjust docs to stop claiming them.
2. **Importer safety:** Bind parameters or escape values correctly, wrap imports in transactions, add schema/type/FK validation, and add a real dry-run mode before touching data.
3. **Documents/templates:** Implement `include_documents` and broaden template/document capture (identify current template/doc directories; ensure binary copy); capture per-file metadata.
4. **Config coverage:** Export/import the wider config set (language packs, themes, units, interconnections, etc.) with filtering for secrets.
5. **Field mapping UX:** Generate mapping suggestions from an emitted mapping file and surface them in preview so administrators can adjust before import.
6. **Background worker alignment:** Decide whether to wire `trigger.php`/`flatten-worker.php` into the UI or drop them; if kept, standardize the produced package to match the main export format and expose download/status in the UI.
7. **Testing & logging:** Add logger usage, error reporting, and basic unit/integration tests around export/import, especially the importer query path and package validation.
8. **Docs cleanup:** Retire/point old export-import markdowns to this summary once the above gaps are addressed and keep a single source of truth.

## How to use today (with caveats)

- **Export:** As admin, open the Backup page and run the Export form. Resulting ZIPs include tables, schemas, and basic docs but lack mapping files and robust metadata.
- **Import:** Use preview first; be aware inserts currently use unbound placeholders and no transactions—validate on a disposable database before attempting production.
