# HotelDroid Export/Import System

> A robust, language-agnostic data export/import system for HotelDroid - designed to be future-proof and compatible with multiple platforms

**Status:** 🟢 **Design Phase Complete** | **Ready for Implementation** | Phase 1 of 6

---

## Overview

This project creates a **parallel export/import system** that operates independently from HotelDroid's existing backup mechanism. Unlike the legacy backup system, this export/import approach:

✅ **Human-Readable Formats** - JSON/XML instead of PHP serialization  
✅ **Language-Agnostic** - Works with any programming language (C#, Python, Node, etc.)  
✅ **Cross-Platform** - Export on Linux, import on Windows; export on MySQL, import on PostgreSQL  
✅ **Future-Proof** - Ready for migration to Blazor, .NET, or other platforms  
✅ **Robust & Safe** - Validation, dry-run mode, rollback on failure  
✅ **Transparent** - All data in structured ZIP packages with clear documentation  

---

## Quick Start

### For Users (Importing/Exporting Data)
1. Read: [EXPORT_IMPORT_QUICKREF.md](EXPORT_IMPORT_QUICKREF.md) (5 min read)
2. See: [docs/ADMIN_GUIDE.md](hoteldruid/export-import/docs/ADMIN_GUIDE.md) (when available)

### For Developers (Contributing to Implementation)
1. Read: [EXPORT_IMPORT_DESIGN.md](EXPORT_IMPORT_DESIGN.md) (20 min read) - Architecture overview
2. Track: [EXPORT_IMPORT_LOG.md](EXPORT_IMPORT_LOG.md) (ongoing) - Progress by session
3. Reference: [EXPORT_IMPORT_SESSION1_SUMMARY.md](EXPORT_IMPORT_SESSION1_SUMMARY.md) (10 min read) - What's been done

### For Future Platform Migration (Blazor, etc.)
1. Read: [EXPORT_IMPORT_DESIGN.md](EXPORT_IMPORT_DESIGN.md) section 3 - Data schema format
2. Reference: [hoteldruid/export-import/samples/](hoteldruid/export-import/samples/) - Example implementations

---

## Project Status

### ✅ Completed (Session 1)
- [x] Architecture designed
- [x] Directory structure specified
- [x] JSON schema formats defined
- [x] Zip package format specified
- [x] 15-session implementation roadmap created
- [x] Progress tracking system established

### 🟡 In Progress (Session 2 - Ready to Start)
- [ ] Directory structure created
- [ ] Base files and stubs created
- [ ] Schema templates created

### 📋 Planned (Sessions 3-15)
- [ ] Core libraries built
- [ ] UI integrated
- [ ] Testing completed
- [ ] Documentation finalized

**Total Progress:** 6.7% (Session 1 of 15) | ~1.5 hours invested | ~10 hours remaining

---

## Key Features

### Export
- **Full System Export** - Complete database + configurations + templates
- **Config-Only Export** - Just settings without data
- **Data-Only Export** - Just database without configs
- **Selective Export** - Specific tables or date ranges (planned)
- **Dry-Run Mode** - Validate before exporting

### Import
- **Full System Import** - Replace everything
- **Merge Import** - Combine with existing data
- **Partial Import** - Specific tables only
- **Preview Mode** - See what will be imported
- **Automatic Rollback** - Undo on any error
- **Cross-Version Support** - Import from different HotelDroid versions

### Safety Features
- **Pre-Import Validation** - Check compatibility before modifications
- **Transaction Support** - All-or-nothing guarantees
- **Integrity Checksums** - Verify data wasn't corrupted
- **Audit Logging** - Track what was imported and when
- **Rollback Capability** - Restore previous state if needed

---

## File Structure

```
HotelDroid/
├── EXPORT_IMPORT_DESIGN.md              ← Architecture specification
├── EXPORT_IMPORT_LOG.md                 ← Progress tracking
├── EXPORT_IMPORT_QUICKREF.md            ← Quick reference guide
├── EXPORT_IMPORT_SESSION1_SUMMARY.md    ← What was accomplished
│
└── hoteldruid/
    ├── export-import/                   ← NEW system (when built)
    │   ├── lib/                         ← PHP libraries
    │   ├── schemas/                     ← JSON schema definitions
    │   ├── ui/                          ← UI integration
    │   ├── tests/                       ← Test suite
    │   ├── docs/                        ← Developer documentation
    │   └── samples/                     ← Reference implementations
    │
    ├── crea_backup.php                  ← Original (unchanged)
    └── includes/funzioni_backup.php     ← Original (unchanged)
```

---

## Architecture Overview

### Parallel System Design
The export/import system runs **completely independently** from the existing backup:

```
┌─────────────────────────────────────────────────────────┐
│  HotelDroid Application                                 │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  EXISTING (unchanged):          NEW (parallel):         │
│  ┌──────────────────┐          ┌──────────────────┐    │
│  │  crea_backup.php │          │ export-import/   │    │
│  │  funzioni_...php │          │ lib/Exporter.php │    │
│  │  (XML, risky)    │          │ (JSON, robust)   │    │
│  └──────────────────┘          └──────────────────┘    │
│         ↓                              ↓                │
│    Old Format              New Format (Zip + JSON)     │
│    (PHP-specific)          (Language-agnostic)         │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Data Transformation
```
Database                Config Files           Export Package
    ↓                        ↓                        ↓
[30+ Tables] ──────→ DataFlattener      ──→ [Structured JSON]
[Relationships]      ConfigExtractor         [Metadata]
[Templates]          ──────────────────      [Schemas]
                                        ─→ [Zip Container]
                                             ↓
                                        Import Package
                                             ↓
                                        New Database/System
```

---

## Implementation Plan

### Phase 1: Foundation (Sessions 1-2) 🟢 ACTIVE
Create directory structure and base infrastructure

### Phase 2: Core Libraries (Sessions 3-7)
Build data flattening, configuration extraction, zip creation

### Phase 3: UI Integration (Sessions 8-9)
Add export/import buttons to web interface

### Phase 4: Import Engine (Sessions 10-11)
Build import logic with validation and rollback

### Phase 5: Testing (Sessions 12-13)
Comprehensive tests and reference implementations

### Phase 6: Documentation & Migration (Sessions 14-15)
Complete documentation and Blazor migration kit

**Estimated Total Time:** 11 hours across multiple sessions  
**Session Frequency:** 1-2 sessions per work day

---

## Documentation

### For Architecture Understanding
- **[EXPORT_IMPORT_DESIGN.md](EXPORT_IMPORT_DESIGN.md)** - Complete system design, schemas, formats

### For Development Progress
- **[EXPORT_IMPORT_LOG.md](EXPORT_IMPORT_LOG.md)** - Session-by-session tracking and todos
- **[EXPORT_IMPORT_QUICKREF.md](EXPORT_IMPORT_QUICKREF.md)** - Quick navigation and reference

### For This Session
- **[EXPORT_IMPORT_SESSION1_SUMMARY.md](EXPORT_IMPORT_SESSION1_SUMMARY.md)** - What was completed in Session 1

### Additional Docs (To Be Created)
- Developer API reference
- Administrator guide
- Blazor migration kit
- Schema reference
- Troubleshooting guide

---

## Data Formats

### Export Package Format (ZIP)
```
export_hoteldruid_20251216_143000_v1.zip
├── manifest.json              ← Start here
├── metadata/
│   ├── export_metadata.json
│   ├── schema_versions.json
│   └── compatibility_info.json
├── schemas/
│   ├── relationships.json
│   └── tables/
│       ├── clienti.table.json
│       ├── contratti.table.json
│       └── ...
├── configs/
│   ├── configurations.json
│   └── templates/
├── data/
│   ├── tables/
│   │   ├── clienti.json
│   │   ├── contratti.json
│   │   └── ...
│   └── relationships.json
├── docs/
│   ├── EXPORT_INFO.txt
│   ├── IMPORT_GUIDE.txt
│   └── SCHEMA_REFERENCE.txt
└── checksums.sha256
```

### Data Format (JSON)
```json
{
  "table_name": "clienti",
  "row_count": 892,
  "rows": [
    {
      "idclienti": 1,
      "cognome": "Rossi",
      "email": "mario@example.com"
    }
  ]
}
```

---

## Key Differences from Existing Backup

| Feature | Old Backup | Export/Import |
| - | - | - |
| **Format** | XML | JSON/XML |
| **Human-Readable** | ❌ | ✅ |
| **Language Support** | PHP only | Any language |
| **Cross-System** | Often fails | ✅ Designed for it |
| **Validation** | Limited | Comprehensive |
| **Rollback** | ❌ | ✅ |
| **Dry-Run Mode** | ❌ | ✅ |
| **Future-Proof** | ❌ | ✅ |

---

## Getting Started

### If You're Starting Development

1. **Read the architecture** (20 min)
   ```bash
   cat EXPORT_IMPORT_DESIGN.md
   ```

2. **Understand current progress** (5 min)
   ```bash
   cat EXPORT_IMPORT_SESSION1_SUMMARY.md
   ```

3. **Start Session 2** (30-45 min)
   - Open `EXPORT_IMPORT_LOG.md`
   - Go to "Session 2" section
   - Follow the checklist
   - Update progress log

### If You're Using the System

Waiting for sessions 3-11 to be completed. You'll see:
1. Export button in HotelDroid interface
2. Import button in HotelDroid interface
3. Documentation on how to use them

---

## Questions?

### Design Questions
See: [EXPORT_IMPORT_DESIGN.md](EXPORT_IMPORT_DESIGN.md) section "Questions for Future Sessions"

### Progress Questions
See: [EXPORT_IMPORT_LOG.md](EXPORT_IMPORT_LOG.md) current session

### How-To Questions
See: [EXPORT_IMPORT_QUICKREF.md](EXPORT_IMPORT_QUICKREF.md)

---

## Contributing

This project follows a **structured, session-based approach**:

1. Each session = specific deliverables (see [EXPORT_IMPORT_LOG.md](EXPORT_IMPORT_LOG.md))
2. Sessions done sequentially (not in parallel)
3. Progress tracked in detail
4. All changes documented immediately

**To contribute:**
1. Review current session in log
2. Complete assigned tasks
3. Update log with progress
4. Commit with clear message
5. Move to next session

---

## Timeline

- **Session 1:** ✅ Complete (Design) - 45 min
- **Sessions 2-4:** Foundation & Structure - 2-3 hours
- **Sessions 5-7:** Core Libraries - 3 hours
- **Sessions 8-11:** UI & Import - 2.5 hours
- **Sessions 12-15:** Testing & Documentation - 2 hours

**Total Estimated Time:** ~11 hours spread across multiple sessions

---

## Project Goals

### ✅ Primary Goals
- Create robust export/import independent from legacy system
- Make formats human-readable and language-agnostic
- Enable safe cross-platform data migration
- Build foundation for future platform migration

### 🎯 Success Criteria
- Export accurately captures all data
- Import works cross-system (MySQL→PostgreSQL, Linux→Windows, etc.)
- No data loss during import
- Rollback works on failure
- At least 95% test coverage
- Complete documentation

### 📚 Knowledge Transfer
- Reference implementations in C# and Python
- Blazor migration kit for future platform
- Detailed documentation for developers
- Architecture preserved for future systems

---

## License & Attribution

This export/import system is designed for HotelDroid (GPLv3).

---

## Status Timeline

- **2025-12-16** - Session 1 Complete: Design & Architecture
- **2025-12-XX** - Session 2: Create directory structure
- **2025-12-XX** - Sessions 3-7: Build core libraries
- **2025-12-XX** - Sessions 8-11: Build UI & import
- **2025-01-XX** - Sessions 12-15: Testing & documentation
- **2025-01-XX** - 🚀 Ready for production use

---

**Project Version:** 1.0.0 (Design Phase)  
**Last Updated:** 2025-12-16 15:00:00  
**Status:** ✅ READY FOR IMPLEMENTATION  
**Next Phase:** Session 2 - Directory Structure Creation  

---

## Quick Links

📖 [Design Document](EXPORT_IMPORT_DESIGN.md)  
📋 [Progress Log](EXPORT_IMPORT_LOG.md)  
🚀 [Quick Reference](EXPORT_IMPORT_QUICKREF.md)  
📊 [Session 1 Summary](EXPORT_IMPORT_SESSION1_SUMMARY.md)  

---

*For questions or suggestions, refer to the design document sections "Questions for Future Sessions"*
