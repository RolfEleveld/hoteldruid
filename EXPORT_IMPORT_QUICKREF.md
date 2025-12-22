# Export/Import - Quick Reference

## 🎉 Implementation Complete!

**Status**: ✅ Production Ready - December 19, 2025

The export/import system with field mapping has been fully implemented and integrated into crea_backup.php.

---

## For Users: How to Use

### Export Data
```
1. Settings → Backup page
2. Find "📤 Export Data" section
3. Check options you want:
   ☑ Include configurations
   ☑ Include templates
   ☑ Include documents
4. Click "Create Export Package"
5. Download the ZIP file
```

### Import Data
```
1. Settings → Backup page
2. Find "📥 Import Data" section
3. Select your export ZIP file
4. Choose mode:
   • Preview first (recommended)
   • Import directly
5. Click "Continue"
6. Review field mappings
7. Click "Confirm Import"
```

---

## What Gets Exported ✅

| Item | Exported | Notes |
|------|----------|-------|
| Database tables | ✅ | All tables as JSON |
| Configurations | ✅ opt | Language, themes, settings |
| Templates | ✅ opt | Document templates |
| Metadata | ✅ | Source, version, timestamp |
| **Not Exported** ❌ | | |
| Passwords | ❌ | Security: never exported |
| API Keys | ❌ | Security: excluded |
| Secrets | ❌ | Security: protected |

---

## Package Structure

```
export_hoteldruid_20251219_143000_prod_v1.zip
├── manifest.json              (What, where, when)
├── data/
│   └── tables/
│       ├── clienti.json
│       ├── contratti.json
│       └── ...
├── metadata/
│   ├── export_metadata.json
│   └── entity_mapping.json    (Field translation!)
├── configs/
│   ├── configurations.json
│   └── templates/
├── schemas/
└── docs/
    ├── EXPORT_INFO.txt
    └── IMPORT_GUIDE.txt
```

---

## Field Mapping

### Automatic
System suggests mappings based on column names:
```
Source          Target
cognome    →    cognome
nome       →    nome
email      →    email
```

### Manual (in Preview)
Adjust any mapping:
```
Source Field → [Target Field Input Box]
cognome      → cognome  (or any other)
nome         → nome
```

### Cross-Platform (International Names)
```
Italian Name    →    International
clienti              guests
contratti            contracts
appartamenti         properties
```

---

## Common Workflows

### 1. Backup & Archive
```
① Create export
② Download ZIP
③ Store safely (cloud, external drive)
④ Label with date
```

### 2. Migrate Systems
```
① Export from Source system
② Download package
③ Move to Target system
④ Import with preview
⑤ Review and confirm
```

### 3. Test Import (Safe!)
```
① Export data
② Select "Preview" mode
③ Review field mappings
④ ✅ No data modified yet
⑤ Can adjust and retry
⑥ Cancel if needed
```

---

## System Requirements

- **Access**: Admin only (User ID 1)
- **PHP**: 7.4+
- **Database**: PostgreSQL, MySQL, SQLite
- **Storage**: 2x database size

---

## Troubleshooting

### Export fails
- ✅ Check: Are you logged in as admin?
- ✅ Check: Is `/export-import/packages` writable?
- ✅ Check: Is database connected?

### Package won't import
- ✅ Check: Is ZIP file not corrupted?
- ✅ Check: Does it contain `manifest.json`?
- ✅ Check: Use preview mode to see error

### Field mapping looks wrong
- ✅ Use suggestions as starting point
- ✅ Adjust in preview mode (safe!)
- ✅ Check entity_mapping.json in package

---

## File Locations

```
hoteldruid/
├── export-import/
│   ├── lib/
│   │   ├── DataFlattener.php
│   │   ├── Exporter.php
│   │   ├── Importer.php
│   │   └── ExportImportUI.php
│   ├── packages/              ← Generated exports
│   ├── README.md              ← Full docs
│   └── .gitignore
│
├── EXPORT_IMPORT_IMPLEMENTATION.md  ← Implementation guide
├── EXPORT_IMPORT_SUMMARY.md         ← This summary  
└── crea_backup.php                  ← Integration point
```

---

## Documentation Files

| File | Purpose | Read Time |
|------|---------|-----------|
| `/export-import/README.md` | Complete system reference | 15 min |
| `EXPORT_IMPORT_IMPLEMENTATION.md` | How it was built | 10 min |
| `EXPORT_IMPORT_SUMMARY.md` | Technical overview | 10 min |
| `EXPORT_IMPORT_QUICKREF.md` | This file! | 5 min |

---

## API for Developers

### Export
```php
include_once('./export-import/lib/Exporter.php');

$exporter = new Exporter($conn, $prefix, $db_type, $dir);
$package = $exporter->createExportPackage([
    'include_configs' => true,
    'export_type' => 'full'
]);
```

### Import
```php
include_once('./export-import/lib/Importer.php');

$importer = new Importer($conn, $prefix, $db_type, $file);
$valid = $importer->validatePackage();
$preview = $importer->getImportPreview();
$stats = $importer->importData($dry_run = false);
```

See `/export-import/README.md` for full API docs.

---

## Pro Tips ⭐

✅ **Do This**:
- [ ] Use preview before importing
- [ ] Download and backup exports
- [ ] Test on non-production first
- [ ] Store exports in secure location
- [ ] Review field mappings carefully

❌ **Don't Do This**:
- [ ] Import without preview
- [ ] Use unverified packages
- [ ] Import to production untested
- [ ] Trust unknown ZIP files

---

## What Works Right Now ✅

- [x] Export full database to JSON
- [x] Create ZIP packages with metadata
- [x] Preview import before applying
- [x] Field mapping with suggestions
- [x] Entity translation support
- [x] Configuration export/import
- [x] Template preservation
- [x] Complete data validation
- [x] Admin access control
- [x] Full UI integration

---

## Key Features

### Export Engine
- Flattens database to portable JSON
- Creates ZIP packages automatically
- Tracks export source and timestamp
- Generates UUIDs for tracing
- Exports configurations & templates

### Import Engine
- Validates package structure
- Generates safe preview (no data modified)
- Suggests field mappings
- Supports manual remapping
- Validates data before insertion

### Field Mapping
- Automatic suggestions
- Manual adjustment UI
- Entity translation support
- Cross-platform compatibility
- Per-field customization

---

## Testing Checklist

- [ ] **Export**: Create test export, download package
- [ ] **Package**: Open ZIP, check manifest.json
- [ ] **Preview**: Import with preview, check data
- [ ] **Mapping**: Verify field mappings look correct
- [ ] **Confirm**: Click "Confirm Import", verify data
- [ ] **Validate**: Check imported data is accurate

---

## Support

**Need help?**
1. Read `/export-import/README.md` - Complete guide
2. Check UI error messages
3. Review package contents manually
4. Contact your system administrator

**Found a bug?**
1. Check `/export-import/README.md` troubleshooting
2. Review PHP error logs
3. Report with:
   - What you did
   - What error you got
   - What you expected

---

## Version Info

- **Component**: Export/Import System v1.0.0
- **Format**: JSON + ZIP (portable)
- **Database Support**: PostgreSQL, MySQL, SQLite  
- **Status**: ✅ Production Ready
- **Implementation Date**: December 19, 2025

---

**Quick Links**:
- 📖 Full docs: `/export-import/README.md`
- 🛠️ Implementation: `EXPORT_IMPORT_IMPLEMENTATION.md`
- 📊 Summary: `EXPORT_IMPORT_SUMMARY.md`
- ⚡ This file: `EXPORT_IMPORT_QUICKREF.md` (you are here)
|------|---------|------|
| `EXPORT_IMPORT_DESIGN.md` | Complete system architecture and specifications | 400+ lines |
| `EXPORT_IMPORT_LOG.md` | Session tracking, progress log, current status | 300+ lines |
| `EXPORT_IMPORT_QUICKREF.md` | This file - quick navigation |

## Architecture at a Glance

```
NEW System (Parallel to existing backup):
/hoteldruid/export-import/
├── lib/                 ← PHP libraries (Exporter, Importer, etc)
├── schemas/             ← JSON schema definitions
├── ui/                  ← Integration with crea_backup.php
├── tests/               ← Comprehensive test suite
├── docs/                ← Developer documentation
└── samples/             ← Example implementations (C#, Python)
```

## Session Overview

### ✅ Session 1 (COMPLETE - 45 min)
**Goal:** Design and document architecture
- Created comprehensive EXPORT_IMPORT_DESIGN.md
- Defined all specifications and formats
- Designed directory structure
- Created 15-step implementation roadmap

### 📋 Session 2 (NEXT)
**Goal:** Create directory structure and base files
- Create `/hoteldruid/export-import/` directories
- Create base PHP class stubs
- Create schema template files
- **Time:** 30-45 minutes

### 📋 Sessions 3-15 (PLANNED)
See `EXPORT_IMPORT_LOG.md` for detailed breakdown

## Key Design Decisions

1. **Parallel System:** Completely separate from existing backup
   - Old system stays untouched
   - New system runs independently

2. **Flattened Data Structure:** Database relationships stored in separate tables
   - Easier to reconstruct in any database
   - Better for cross-language support

3. **JSON Primary Format:** More portable than XML
   - Better future support for C#/.NET
   - Compatible with all languages

4. **Zip Container:** Versioned, structured format
   - Allows internal directory structure
   - Easy to extend with new content

5. **Metadata-First:** Validation before data modification
   - Check compatibility before importing
   - Prevents data corruption

## Quick Links

- **Architecture Details:** See `EXPORT_IMPORT_DESIGN.md` sections 2-5
- **Implementation Plan:** See `EXPORT_IMPORT_DESIGN.md` section 7
- **Progress Tracking:** See `EXPORT_IMPORT_LOG.md` Sessions 1-15
- **File Structure:** See `EXPORT_IMPORT_DESIGN.md` section 4

## What's Different from Old Backup System

| Aspect | Old Backup | NEW Export/Import |
|--------|-----------|-------------------|
| **Format** | XML (hard to parse) | JSON/XML (easy) |
| **Portability** | PHP-specific | Language-agnostic |
| **Structure** | Flat XML | Organized Zip |
| **Metadata** | Minimal | Comprehensive |
| **Cross-system** | ❌ Often fails | ✅ Designed for it |
| **Rollback** | ❌ No | ✅ Yes |
| **Validation** | ❌ Limited | ✅ Full |
| **Future migration** | ❌ Locked to PHP | ✅ Ready for Blazor |

## Starting Next Session

1. Open `/hoteldruid/EXPORT_IMPORT_LOG.md`
2. Go to "Session 2: [PENDING]"
3. Follow the "To Do" checklist
4. Update the log as you complete items
5. Commit changes when session is complete

## Estimated Total Timeline

- **Sessions 1-2** (Foundation): ✅ + 45 min = **1.5 hours**
- **Sessions 3-7** (Core Libraries): ~3 hours
- **Sessions 8-11** (UI & Import): ~2.5 hours
- **Sessions 12-13** (Testing & References): ~2 hours
- **Sessions 14-15** (Docs & Migration): ~2 hours

**Total Estimated Time:** 11 hours across multiple sessions

## Important Notes

- ⚠️ **Do NOT modify** `/hoteldruid/crea_backup.php` or `/hoteldruid/includes/funzioni_backup.php` in early sessions
- 📝 **Always update** `EXPORT_IMPORT_LOG.md` at end of session
- 📦 **No rushing** - each session builds on previous, test thoroughly
- 🎯 **Stay focused** - follow one session at a time

## Questions to Consider

1. Should we support partial imports (specific tables only)?
2. Should export support filtering (date ranges, specific records)?
3. Include file uploads or just database?
4. Importer dry-run mode?
5. Track audit logs of all imports?

---

**Created:** 2025-12-16  
**Next Update:** After Session 2 completion
