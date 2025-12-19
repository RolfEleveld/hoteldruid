# HotelDroid Export/Import System - Design Document

**Project Goal:** Create a parallel, robust export/import system that operates independently from the existing backup/restore mechanism. The system should be human-readable, language-agnostic, and future-proof for migration to other platforms (e.g., Blazor).

**Status:** Planning Phase  
**Started:** 2025-12-16  
**Last Updated:** 2025-12-16

---

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Core Principles](#core-principles)
3. [System Design](#system-design)
4. [Data Schema](#data-schema)
5. [Zip Package Format](#zip-package-format)
6. [Implementation Roadmap](#implementation-roadmap)
7. [Progress Log](#progress-log)

---

## Architecture Overview

### High-Level Design

```text
HotelDroid Application
├── Existing Backup/Restore (UNCHANGED)
│   ├── crea_backup.php (original)
│   └── funzioni_backup.php (original)
│
└── NEW: Export/Import System (PARALLEL)
    ├── UI Layer (crea_backup.php extended UI)
    ├── PHP Export Engine (export-import/lib/)
    ├── PHP Import Engine (export-import/lib/)
    ├── Validation Layer (export-import/lib/validators/)
    ├── Schema Definitions (export-import/schemas/)
    └── Generated Packages (export-import/packages/)
```

### Design Philosophy

1. **Independence:** Completely separate from legacy backup system - both can coexist
2. **Portability:** No PHP-specific serialization (no serialize(), only JSON/XML)
3. **Transparency:** All data human-readable and editable with standard tools
4. **Versioning:** Built-in schema versioning for forward/backward compatibility
5. **Atomicity:** All-or-nothing imports with rollback capability
6. **Language Agnostic:** JSON/XML formats can be parsed by any language (C#, Node, Python, etc.)
7. **Extensibility:** Structure allows plugins for custom data types

---

## Core Principles

### 1. **Data Flattening**

Instead of nested structures, flatten database relationships:

```json
Traditional (nested):
{
  "customer": {
    "id": 1,
    "name": "Mario Rossi",
    "addresses": [
      { "street": "Via Roma", "city": "Milano" },
      { "street": "Via Torino", "city": "Napoli" }
    ]
  }
}

Flattened (export/import):
{
  "customers": [
    { "id": 1, "name": "Mario Rossi", "rel_count": 2 }
  ],
  "customer_addresses": [
    { "customer_id": 1, "street": "Via Roma", "city": "Milano", "order": 1 },
    { "customer_id": 1, "street": "Via Torino", "city": "Napoli", "order": 2 }
  ]
}
```

**Benefits:**

- Easy to reconstruct in any database
- Clear relationship mapping
- Prevents circular references
- Supports many-to-many relationships naturally

### 2. **JSON-Only Data**

All data and configurations are JSON-based:

- Database tables exported as JSON
- Configuration files converted to JSON
- No PHP serialization or binaries (PHP-agnostic)
- Exception: Template documents (.rtf, .docx) included as binary
- Zip file CRC validation ensures data integrity (no separate hashes needed)

**Benefits:**

- Parseable by any language/platform
- Human-readable and editable
- No PHP dependencies for import
- Future-proof for Blazor/.NET migration

### 3. **Source-Centric Manifest**

Manifest documents the ORIGIN, not the destination:

- Where data came from (hostname, system name, database type)
- When it was exported (timestamp)
- What version created it (application version)
- System identification (optional: MAC address, UEFI hash for audit trail)
- NO target system compatibility list (destination is unknown)

**Benefits:**

- Admin can easily trace where exports originated
- Enables audit trail for compliance
- No need to update manifest for each target system
- Supports cross-system and cross-version imports

### 4. **Human Readability**

- JSON with consistent formatting
- Field names match database columns
- Manifest includes human-readable summary
- Readable file organization inside zip
- Plain text export info files for quick reference

### 5. **Schema Versioning**

Track schema evolution:

```json
{
  "export_schema_version": "1.0.0",
  "min_import_version": "1.0.0",
  "max_import_version": "2.0.0",
  "migrations": [
    { "from": "1.0.0", "to": "1.1.0", "description": "Added contact_method field" }
  ]
}
```

---

## System Design

### Directory Structure

```text
hoteldruid/
└── export-import/
    ├── README.md                    # System documentation
    ├── lib/
    │   ├── Exporter.php            # Main export orchestrator
    │   ├── Importer.php            # Main import orchestrator
    │   ├── ZipBuilder.php          # Creates versioned zip packages
    │   ├── DataFlattener.php       # Flattens database to JSON
    │   ├── ConfigExtractor.php     # Extracts PHP config to JSON
    │   ├── validators/
    │   │   ├── SchemaValidator.php # Validates against schema
    │   │   ├── DataValidator.php   # Checks data integrity
    │   │   └── CompatibilityValidator.php # Version checking
    │   └── utils/
    │       ├── Logger.php          # Unified logging
    │       ├── FileHelper.php      # File operations
    │       └── JsonHelper.php      # JSON utilities
    ├── schemas/
    │   ├── manifest.schema.json    # Top-level package structure
    │   ├── metadata.schema.json    # Export metadata format
    │   ├── tables/
    │   │   ├── clienti.table.json  # Per-table schemas
    │   │   ├── contratti.table.json
    │   │   └── ...
    │   ├── relationships.schema.json # FK definitions
    │   └── config.schema.json      # Config file format
    ├── ui/
    │   ├── ExportUI.php            # UI hooks for export
    │   ├── ImportUI.php            # UI hooks for import
    │   └── styles.css              # UI styling
    ├── tests/
    │   ├── ExporterTest.php        # Export validation tests
    │   ├── ImporterTest.php        # Import validation tests
    │   ├── SampleData.php          # Test fixtures
    │   └── CrossSystemTest.php     # Multi-machine testing
    ├── samples/                    # Example export packages
    │   └── sample_export_v1.zip    # Sample package for documentation
    ├── packages/                   # Generated packages (gitignored)
    │   └── .gitignore
    └── docs/
        ├── ARCHITECTURE.md         # Detailed architecture
        ├── ZIP_FORMAT.md           # Zip package specification
        ├── JSON_SCHEMA.md          # Schema reference
        ├── ADMIN_GUIDE.md          # User guide
        ├── API_REFERENCE.md        # Developer API
        ├── MIGRATION_BLAZOR.md     # Blazor port guidelines
        └── CHANGELOG.md            # Version history
```

### Component Responsibilities

| Component | Responsibility | Language |
|-----------|-----------------|----------|
| **Exporter.php** | Orchestrates full export process | PHP |
| **Importer.php** | Orchestrates full import process | PHP |
| **DataFlattener.php** | Converts DB rows to JSON structures | PHP |
| **ConfigExtractor.php** | Converts PHP config files to JSON | PHP |
| **SchemaValidator.php** | Validates data against JSON schemas | PHP |
| **ZipBuilder.php** | Creates proper zip structure | PHP |
| **Reference Importer (.NET)** | Example C# importer for Blazor | C# |
| **Reference Importer (Python)** | Example Python importer for documentation | Python |

---

## Data Schema

### 1. Manifest Structure

**File:** `manifest.json` (root of zip)

Declares the ORIGIN of the export, enabling administrators to trace and audit exports.

```json
{
  "export_format_version": "1.0.0",
  "export_timestamp": "2025-12-16T14:30:00Z",
  "export_id": "uuid-v4-here",
  "source_system": {
    "application": "HotelDroid",
    "version": "3.07",
    "database_type": "postgresql|mysql|sqlite"
  },
  "source_machine": {
    "hostname": "hotel-server-prod-01",
    "system_name": "Main Hotel Database",
    "hosting_system": "PHP 7.4.3",
    "exported_by": "admin@hotel.com",
    "machine_id": {
      "mac_address": "00:1A:2B:3C:4D:5E",
      "uefi_uuid": "550e8400-e29b-41d4-a716-446655440000"
    }
  },
  "export_metadata": "metadata/export_metadata.json",
  "contents": {
    "metadata": "metadata/",
    "configurations": "configs/",
    "data": "data/",
    "schemas": "schemas/"
  },
  "data_summary": {
    "total_tables": 30,
    "total_rows": 145000,
    "format_notes": "All data in JSON format. Zip CRC validates integrity. No separate checksums needed."
  }
}
```

**Notes:**

- `machine_id` fields (MAC, UEFI UUID) are OPTIONAL for simpler deployments
- Manifest does NOT specify target systems or compatibility
- Import system determines compatibility based on schema validation

### 2. Metadata Structure

**File:** `metadata/export_metadata.json`

Detailed information about the export for audit and reference.

```json
{
  "export_id": "uuid-v4-here",
  "export_timestamp": "2025-12-16T14:30:00Z",
  "export_duration_seconds": 23,
  "database_connection": {
    "type": "postgresql",
    "host": "db.example.com",
    "database_name": "hoteldruid_prod"
  },
  "export_scope": {
    "export_type": "full|partial|rooms_only|migration",
    "total_tables": 30,
    "total_rows": 145000,
    "total_config_files": 12,
    "tables_included": {
      "clienti": 892,
      "contratti": 127,
      "prenota2025": 34567,
      "...": "..."
    }
  },
  "export_options": {
    "include_configs": true,
    "include_templates": true,
    "include_documents": true,
    "format": "json",
    "compression": "zip_deflate"
  },
  "data_integrity": {
    "note": "Zip file CRC32 checks integrity. No separate checksums needed.",
    "zip_algorithm": "deflate"
  },
  "notes": "Full backup for migration purposes. Source verified."
}
```

**Notes:**

- Export duration helps troubleshoot performance
- Scope details enable selective re-export if needed
- No per-table or aggregate hashes (Zip CRC is sufficient)
- Document audit trail for compliance

### 3. Table Data Structure

**File:** `data/tables/clienti.json`

```json
{
  "table_name": "clienti",
  "schema_version": "1.0.0",
  "row_count": 892,
  "columns": [
    {
      "name": "idclienti",
      "type": "integer",
      "nullable": false,
      "primary_key": true,
      "comment": "Unique client identifier"
    },
    {
      "name": "cognome",
      "type": "string",
      "length": 70,
      "nullable": false
    },
    {
      "name": "email",
      "type": "string",
      "nullable": true
    }
  ],
  "relationships": [
    {
      "column": "idclienti",
      "related_table": "relclienti",
      "related_column": "idclienti",
      "type": "one_to_many"
    }
  ],
  "rows": [
    {
      "idclienti": 1,
      "cognome": "Rossi",
      "nome": "Mario",
      "email": "mario@example.com",
      "datainserimento": "2020-01-15T10:30:00Z"
    },
    {
      "idclienti": 2,
      "cognome": "Bianchi",
      "nome": "Giovanni",
      "email": null,
      "datainserimento": "2020-02-20T14:15:00Z"
    }
  ],
  "metadata": {
    "exported_count": 892,
    "filtered_out": 0,
    "notes": "No per-table hash. Zip CRC validates integrity."
  }
}
```

### 4. Configuration Structure

**File:** `configs/configurations.json`

All configurations extracted from PHP files and stored as JSON.

```json
{
  "configurations": {
    "lingua": {
      "source_file": "dati/lingua.php",
      "description": "Language settings",
      "data": {
        "1": "ita",
        "2": "en",
        "3": "es"
      }
    },
    "unit": {
      "source_file": "dati/unit.php",
      "description": "Unit measurement settings",
      "data": {
        "s_n": "$trad_var['single']",
        "p_n": "$trad_var['plural']",
        "gender": "0"
      }
    },
    "tema": {
      "source_file": "dati/tema.php",
      "description": "UI theme settings",
      "data": {
        "1": "snj",
        "2": "blu",
        "parole_sost": 0
      }
    },
    "selectappartamenti": {
      "source_file": "dati/selectappartamenti.php",
      "description": "Apartment selection list",
      "data": "apartment_1\napartment_2\napartment_3"
    }
  },
  "templates": {
    "included": ["mdl_disponibilita.rtf", "mdl_contratto.rtf"],
    "location": "../templates/",
    "description": "Document templates are included as-is (binary format)"
  },
  "metadata": {
    "total_config_files": 12,
    "files_exported": 11,
    "files_skipped": 1,
    "skipped_reason": "abilita_login file not found"
  }
}
```

**Notes:**

- All configuration values converted to JSON
- Template documents (.rtf, .docx) preserved as binary files
- Importer reconstructs PHP config files from JSON during import
- Unknown config keys are preserved for forward/backward compatibility

### 5. Entity Mapping Structure

**File:** `metadata/entity_mapping.json`

Maps Italian/implementation-specific entity names to international identifiers, enabling different implementations to import the same package.

```json
{
  "entity_mapping": {
    "table_translations": {
      "clienti": {
        "international_name": "guests",
        "description": "Client/Guest records",
        "primary_key": "idclienti",
        "record_count": 892
      },
      "contratti": {
        "international_name": "contracts",
        "description": "Client contracts",
        "primary_key": "idcontratti",
        "record_count": 127
      },
      "appartamenti": {
        "international_name": "properties",
        "description": "Apartment/Room definitions",
        "primary_key": "idappartamenti",
        "record_count": 45
      },
      "prenota2025": {
        "international_name": "bookings_2025",
        "description": "Bookings for year 2025",
        "primary_key": "idprenota",
        "record_count": 34567,
        "yearly": true
      },
      "regole": {
        "international_name": "rules",
        "description": "Booking rules and restrictions",
        "primary_key": "id",
        "record_count": 156
      },
      "ntariffe": {
        "international_name": "rates",
        "description": "Nightly rates/pricing",
        "primary_key": "id",
        "record_count": 892
      }
    },
    "field_translations": {
      "clienti": {
        "idclienti": "id",
        "cognome": "last_name",
        "nome": "first_name",
        "email": "email_address",
        "datainserimento": "created_date",
        "indirizzo": "address",
        "citta": "city",
        "cap": "postal_code",
        "provincia": "province",
        "nazione": "country"
      },
      "contratti": {
        "idcontratti": "id",
        "idclienti": "guest_id",
        "descrizione": "description",
        "data_inizio": "start_date",
        "data_fine": "end_date",
        "importo": "amount"
      }
    },
    "relationship_translations": {
      "clienti_idclienti": {
        "source_table": "clienti",
        "source_table_international": "guests",
        "source_key": "idclienti",
        "source_key_international": "id"
      },
      "contratti_idclienti": {
        "source_table": "contratti",
        "source_table_international": "contracts",
        "source_key": "idclienti",
        "source_key_international": "guest_id",
        "references_table": "clienti",
        "references_table_international": "guests",
        "references_key": "idclienti",
        "references_key_international": "id"
      }
    }
  },
  "implementation_notes": {
    "export_implementation": "HotelDroid-PHP",
    "export_locale": "it-IT",
    "export_version": "3.07",
    "mapping_version": "1.0",
    "description": "Complete mapping for Italian HotelDroid to international entity model"
  }
}
```

**Purpose:**

- Enables imports to different implementations (PHP, Blazor, etc.)
- Allows target system to map international names back to internal names
- Preserves data relationships across different naming conventions
- Supports localized implementations (each language, each system type)

**How Importers Use This:**

1. Read entity_mapping.json
2. Check if target system recognizes international names
3. If not, apply reverse mapping to convert:
   - Table names: `guests` → `clienti` (or to whatever target uses)
   - Field names: `last_name` → `cognome` (or target equivalent)
   - Relationships: Use mapping to validate foreign keys
4. Import with transformed names

**Example Import Scenarios:**

**Scenario 1**: Same System (PHP)

```text
Export: Table "clienti" with field "cognome"
Import to PHP HotelDroid: 
  - Read mapping: "clienti" = international "guests"
  - Recognize: Our internal name IS "clienti"
  - Skip transformation (already native format)
  - Import directly
```

**Scenario 2**: Different Implementation (Blazor/.NET)

```text
Export: Table "clienti" with field "cognome"
Import to Blazor system:
  - Read mapping: "clienti" = international "guests"
  - Check internal schema: we use "Customer" table
  - Apply transformation rules:
    - Table "guests" → "Customer"
    - Field "cognome" → "LastName"
  - Transform data accordingly
  - Import
```

**Scenario 3**: Future Implementation

```text
Any future HotelDroid implementation:
  - Uses same mapping structure
  - Defines own transformation layer
  - Can import from any version/language
```

- Unknown config keys are preserved for forward/backward compatibility

---

## Zip Package Format

### Structure

```text
export_hoteldruid_20251216_143000_prod_v1.zip
├── manifest.json                    ← START HERE (source info)
├── metadata/
│   ├── export_metadata.json         (export details)
│   ├── schema_versions.json         (schema tracking)
│   └── entity_mapping.json          (table/field translations)
├── schemas/
│   ├── relationships.json           (FK mappings)
│   └── tables/
│       ├── clienti.schema.json
│       ├── contratti.schema.json
│       ├── prenota2025.schema.json
│       └── ... (all tables)
├── configs/
│   ├── configurations.json          (all PHP configs as JSON)
│   └── templates/
│       ├── mdl_disponibilita.rtf    (binary: doc templates)
│       ├── mdl_contratto.docx
│       └── ... (RTF/DOCX only)
├── data/
│   ├── tables/
│   │   ├── clienti.json
│   │   ├── contratti.json
│   │   ├── prenota2025.json
│   │   └── ... (all table exports)
│   └── relationships.json           (FK links)
└── docs/
    ├── EXPORT_INFO.txt              (human-readable summary)
    ├── IMPORT_GUIDE.txt             (how to use this package)
    └── SCHEMA_REFERENCE.txt         (table definitions)
```

**CRC Validation:**

- Zip file CRC32 automatically validates integrity on extraction
- No separate checksum files needed
- If extraction succeeds, data is uncorrupted

### Naming Convention

Packages are named consistently for easy identification:

```text
export_hoteldruid_{YYYYMMDD}_{HHMMSS}_{SOURCE_NAME}_{VERSION}.zip

Examples:
- export_hoteldruid_20251216_143000_prod_v1.zip
- export_hoteldruid_20251216_150000_dev_v1.zip
- export_hoteldruid_20251216_145500_backup_migration_v1.zip
```

**Convention:**

- Date: YYYYMMDD format (2025-12-16 → 20251216)
- Time: HHMMSS format (24-hour)
- Source: Short name (prod, dev, backup, migration, etc.)
- Version: Export format version (currently v1)

---

## Implementation Roadmap

### Phase 1: Foundation (Steps 1-4)

- [ ] Design documentation (THIS FILE)
- [ ] Directory structure and base files
- [ ] JSON schema definitions
- [ ] Basic infrastructure

### Phase 2: Export Engine (Steps 5-7)

- [ ] Data flattener library
- [ ] Configuration extractor
- [ ] Zip package generator

### Phase 3: UI Integration (Steps 8-9)

- [ ] Export UI button
- [ ] Validation before export

### Phase 4: Import Engine (Steps 10-11)

- [ ] Importer library
- [ ] Import UI and preview

### Phase 5: Quality Assurance (Steps 12-13)

- [ ] Reference implementations
- [ ] Comprehensive tests

### Phase 6: Documentation & Migration (Steps 14-15)

- [ ] Complete documentation
- [ ] Blazor migration kit

---

## Progress Log

### Session 1: 2025-12-16

- [x] Created EXPORT_IMPORT_DESIGN.md (this file)
- [x] Defined architecture and directory structure
- [x] Designed JSON schema format
- [x] Designed Zip package format
- [x] Created implementation roadmap
- [ ] Next: Create directory structure and base files

**Time Spent:** ~30 minutes  
**Next Session Goal:** Complete Phase 1 - Foundation setup

---

## Key Design Decisions Documented

### Decision 1: Flattened Data Structure

- **Why:** Simpler to reconstruct in any database system
- **Alternative Rejected:** Nested JSON (harder for cross-platform reconstruction)

### Decision 2: Separate from Legacy Backup

- **Why:** Allows existing system to work unchanged, new system is parallel
- **Alternative Rejected:** Modifying existing backup system (high risk of breaking)

### Decision 3: Source-Centric Manifest (No Target Compatibility)

- **Why:** Admin can trace where exports originated. Import system determines compatibility based on schema validation, not pre-declared targets.
- **Rationale:** We don't know where the export will be imported, so declaring target systems is irrelevant. Import validation handles compatibility checking.
- **Alternative Rejected:** Include compatibility list (breaks third-party imports, requires manifest updates for each target)

### Decision 4: Zip CRC Instead of Per-Table Hashes

- **Why:** Simpler, more open ecosystem, allows third-party package creation without knowing our hash algorithms
- **Rationale:** Zip's native CRC32 is sufficient for corruption detection. Schema validation during import catches data inconsistencies.
- **Alternative Rejected:** Multiple SHA256 hashes (opaque, limits interoperability, adds complexity)

### Decision 5: JSON Primary Format

- **Why:** JSON more portable, better support across languages
- **Alternative Considered:** XML (better schema validation, but heavier)

### Decision 6: Zip Container with Template Documents

- **Why:** Standard format, widely supported, can contain structured directories. Template documents (.rtf, .docx) included as-is for user reference.
- **Alternative Considered:** Single JSON file (less extensible for future template additions)

### Decision 7: Human-Readable System Identification

- **Why:** Admins can easily trace exports. Optional machine ID fields (MAC, UEFI) for audit trail without adding complexity to basic deployments.
- **Alternative Rejected:** Anonymous exports (impossible to audit source)

### Decision 8: Metadata-First Design

- **Why:** Enables source verification and audit trail before import. Manifest includes origin info, not target compatibility.
- **Alternative Rejected:** Minimal metadata (can't trace export sources)

### Decision 9: Entity Mapping for Implementation Agnostic Imports

- **Why:** Different implementations (PHP, Blazor, other languages) can use same export packages by mapping Italian table names to their internal schema.
- **Rationale:** Includes entity_mapping.json with table, field, and relationship translations. Importers apply mapping rules needed for their implementation.
- **Benefit:** Package can be imported to unlimited implementations without modification. Each implementation handles its own mapping layer.
- **Alternative Rejected:** Hard-coded table names (only works for same system)

### Decision 10: Implementation-Agnostic Hosting System Field

- **Why:** Changed php_version to hosting_system (e.g., "PHP 7.4.3", "Blazor .NET 8.0", "Node.js 20.0") to support any implementation.
- **Rationale:** manifest.json should indicate HOW something is hosted, not require it to be PHP-specific.
- **Benefit:** Single export package can be created by PHP, imported by Blazor, imported by future Node implementation. No version conflicts.
- **Alternative Rejected:** PHP-specific version field (breaks cross-platform vision)

---

## Data Integrity & Validation

### How Hashes Work in This System

**No Per-Table Hashes:**

- We do NOT calculate and store SHA256 for each table
- Tables are JSON files inside the zip
- Zip's native CRC32 validates each file on extraction
- If extraction succeeds, all data is verified uncorrupted

**Why This Approach:**

- Simpler: no hash calculation overhead
- More open: third parties can create packages without knowing our hash algorithm
- More flexible: partial imports don't need matching aggregate hashes
- More robust: Zip CRC is sufficient and widely supported

**Manifest Hash (Optional):**

- If desired, manifest.json alone can have SHA256
- Useful for download verification (detect transfer corruption)
- But not required for import validation

### Validation Pipeline

**On Export:**

1. Validate database connectivity
2. Check all tables exist
3. Flatten data to JSON (validate types during conversion)
4. Extract configs to JSON
5. Create zip with metadata
6. Zip file automatically creates CRC32

**On Import:**

1. Extract zip (automatic CRC validation)
2. Read manifest (source info verification)
3. Read entity_mapping.json (understand table/field naming)
4. Determine if transformation needed:
   - Same implementation? Skip transformation
   - Different implementation? Apply mapping rules
5. Validate JSON schema compliance (each table matches expected schema)
6. Check referential integrity (FK links exist)
7. Verify data types match definitions
8. Transform entity names if needed (tables, fields, relationships)
9. Apply transformation rules if cross-version
10. Import data into database

---

## Questions for Future Sessions

1. Should we support partial imports (import specific tables only)?
2. Should export support filtering (by date range, specific customers, etc.)?
3. Should importer support dry-run mode (preview before applying)?
4. Should we track audit logs of imports (who, when, what)?
5. Should we support transformation rules for cross-system imports?

---

## Related Files

- `/hoteldruid/crea_backup.php` - Existing backup system (unchanged)
- `/hoteldruid/includes/funzioni_backup.php` - Existing backup functions (unchanged)
- `/hoteldruid/EXPORT_IMPORT_DESIGN.md` - This file

---

**Design Document Version:** 1.0.0  
**Last Updated:** 2025-12-16 14:32:00  
**Status:** Ready for Phase 1 Implementation
