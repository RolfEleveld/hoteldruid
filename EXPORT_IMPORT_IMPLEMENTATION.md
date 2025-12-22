# Export/Import Implementation Guide

## What Has Been Added

A complete, production-ready export/import system has been integrated into your HotelDroid crea_backup.php page. This system provides:

### ✅ Completed Components

1. **Data Export Engine** (`Exporter.php`)
   - Flattens database tables to JSON format
   - Creates portable ZIP packages with metadata
   - Includes configurations and templates
   - Generates UUIDs for package tracking
   - Automatic source documentation

2. **Data Import Engine** (`Importer.php`)
   - Validates package structure and contents
   - Performs schema validation before import
   - Generates data preview for review
   - Supports field mapping with intelligent suggestions
   - Handles data type conversion
   - Rollback capability (transactional imports)

3. **Field Mapping System** (`ExportImportUI.php`)
   - Automatic mapping suggestions
   - Preview of all data before import
   - Per-field customization interface
   - Support for table name translation
   - Entity mapping for cross-platform imports

4. **UI Integration** (`export-import-handlers.php`)
   - Seamlessly integrated into crea_backup.php
   - Export button with options
   - Import upload with preview
   - Field mapping UI
   - Result summary after import

## File Structure

```
hoteldruid/
├── export-import/
│   ├── lib/
│   │   ├── DataFlattener.php        # Table → JSON conversion
│   │   ├── Exporter.php             # Creates ZIP packages
│   │   ├── Importer.php             # Imports with validation
│   │   └── ExportImportUI.php       # UI components
│   ├── packages/                    # Generated export files
│   ├── export-import-handlers.php   # Integration layer
│   ├── README.md                    # Full documentation
│   └── .gitignore
└── crea_backup.php                  # (Modified to include handlers)
```

## User Guide

### For Administrators

#### Exporting Data

1. Navigate to **Settings** → **Backup**
2. Scroll to **📤 Export Data** section
3. Check options:
   - ✓ Include configurations (language, themes, units)
   - ✓ Include templates (document templates)
   - ✓ Include documents (contracts, etc.)
4. Click **Create Export Package**
5. Download the ZIP file that appears

**What Gets Exported:**
- All database tables (clienti, contratti, prenota*, etc.)
- Configuration files (in JSON format)
- Document templates (.rtf, .docx files)
- Metadata about the export (source, timestamp, version)
- Entity mapping for cross-platform compatibility

#### Importing Data

1. Navigate to **Settings** → **Backup**
2. Scroll to **📥 Import Data** section
3. Click on file input to select your export package
4. Choose import mode:
   - **Preview before importing** ← Recommended first time
   - **Import directly** ← For verified packages
5. Check **Import configurations** if needed
6. Click **Continue**

**Preview Mode (Recommended):**
- Review all tables and row counts
- See field mappings with suggestions
- Adjust any mappings if needed
- Click **Confirm Import** when ready

**Data Safety:**
- Admin access required (must be logged in as user ID 1)
- Preview before import prevents accidents
- All data validated before insertion
- Original data preserved during preview

### For Developers

#### Creating Exports Programmatically

```php
<?php
include_once('./export-import/lib/Exporter.php');

// Initialize exporter
$exporter = new Exporter(
    $numconnessione,      // Your DB connection resource
    $PHPR_TAB_PRE,        // Your table prefix (e.g., 'phpr_')
    $PHPR_DB_TYPE,        // Database type ('postgresql', 'mysql', 'sqlite')
    $export_dir           // Where to save packages
);

// Create package with options
$package = $exporter->createExportPackage([
    'include_configs' => true,
    'include_templates' => true,
    'export_type' => 'full',
    'source_name' => 'production'
]);

// Use the package file
if ($package) {
    echo "Created: " . $package;
} else {
    echo "Export failed";
}
?>
```

#### Importing Data Programmatically

```php
<?php
include_once('./export-import/lib/Importer.php');

// Initialize importer
$importer = new Importer(
    $numconnessione,
    $PHPR_TAB_PRE,
    $PHPR_DB_TYPE,
    '/path/to/export.zip'
);

// Validate before import
$validation = $importer->validatePackage();
if (!$validation['valid']) {
    print_r($validation['errors']);
    exit;
}

// Get preview (optional)
$preview = $importer->getImportPreview($max_rows = 5);
foreach ($preview as $table => $info) {
    echo "$table: " . $info['row_count'] . " rows\n";
}

// Set custom field mappings (optional)
$importer->setFieldMapping('clienti', 'clienti', [
    'idclienti' => 'idclienti',
    'cognome' => 'cognome',
    'nome' => 'nome',
    'email' => 'email'
]);

// Execute import
$stats = $importer->importData(
    $dry_run = false,      // Set to true to preview only
    $import_configs = true // Include config files
);

// Check results
echo "Tables processed: " . $stats['tables_processed'] . "\n";
echo "Rows imported: " . $stats['rows_imported'] . "\n";
if (!empty($stats['errors'])) {
    echo "Errors: " . print_r($stats['errors'], true);
}
?>
```

#### Working with Field Mapping

```php
<?php
// Get mapping suggestions from entity mapping
$suggestions = $importer->getMappingSuggestions();

foreach ($suggestions as $source_table => $suggestion) {
    echo "Table: " . $source_table . " → " . 
         $suggestion['international_name'] . "\n";
    
    foreach ($suggestion['suggested_field_mapping'] as $source => $target) {
        echo "  $source → $target\n";
    }
}

// Apply custom mapping
$importer->setFieldMapping('clienti', 'customers', [
    'idclienti' => 'customer_id',
    'cognome' => 'last_name',
    'nome' => 'first_name',
    'email' => 'email_address'
]);
?>
```

## Package Structure

### Package Filename
```
export_hoteldruid_20251219_143000_prod_v1.zip
                └─ Date ─┘└─ Time ─┘ └─Source┘
```

### Contents

```
export_hoteldruid_*.zip
├── manifest.json                  # Source information
├── metadata/
│   ├── export_metadata.json      # Export details
│   └── entity_mapping.json       # Table/field translations
├── data/
│   └── tables/
│       ├── clienti.json         # One file per table
│       ├── contratti.json
│       ├── appartamenti.json
│       └── ...
├── configs/
│   ├── configurations.json       # Settings in JSON
│   └── templates/                # .rtf, .docx files
├── schemas/
│   ├── relationships.json        # FK definitions
│   └── tables/                   # Schema for each table
└── docs/
    ├── EXPORT_INFO.txt          # Human-readable summary
    ├── IMPORT_GUIDE.txt         # Instructions
    └── SCHEMA_REFERENCE.txt     # Field definitions
```

## Security & Safety

### Access Control ✅
- **Admin Only**: Only user ID 1 can export/import
- **Authentication Required**: Full HotelDroid login
- **Permission Integrated**: Uses existing privilege system

### Data Protection ✅
- **No Passwords**: Database passwords never exported
- **No User Secrets**: Authentication data excluded
- **No Sensitive Config**: API keys excluded
- **Audit Trail**: Export source and timestamp recorded

### Import Safety ✅
- **Preview First**: Review before importing
- **Validation**: Complete package validation
- **Type Checking**: Data types verified
- **Referential Integrity**: FK constraints checked
- **Cleanup**: Temporary files automatically deleted

## Troubleshooting

### Export Not Working
- Check that you're logged in as admin (user ID 1)
- Verify `/export-import/packages` directory is writable
- Check PHP error logs for details

### Import Package Invalid
- Ensure ZIP wasn't corrupted during download
- Verify it was created by this system
- Check manifest.json exists in package

### Field Mapping Issues
- Column names must match exactly
- Verify target table exists
- Check entity_mapping.json for suggestions

## What Data Gets Handled

### Exported & Imported ✅
- All database tables
- Row data with proper type conversion
- Column definitions
- Configuration files (as JSON)
- Document templates

### Not Exported ❌
- Database connection passwords
- User login credentials
- Session tokens
- API keys
- Sensitive system files

### Optional Export
- Configuration files (enable with checkbox)
- Template documents (enable with checkbox)
- Documents/contracts (enable with checkbox)

## Integration with Existing Backup System

The export/import system operates **completely independently** from the existing backup system:

- **Backup System**: Creates PHP-serialized backup files
  - Traditional HotelDroid format
  - Complete database dump
  - XML-like structure
  - Can include/exclude years

- **Export/Import System**: Creates JSON ZIP packages
  - Modern portable format
  - Language-agnostic
  - With field mapping support
  - Runs parallel to backup

**Both systems can coexist** - use whichever best fits your needs!

## Next Steps

1. **Test Export**: Create an export package to see the structure
2. **Review Package**: Open the ZIP and examine manifest.json
3. **Test Import**: Import into a test system
4. **Validate Data**: Confirm all data imported correctly
5. **Automate**: Create scripts for scheduled exports

## Support & Documentation

- 📖 **README.md**: Full system documentation in `/export-import/`
- 💾 **crea_backup.php**: Integration point and UI handler
- 🏗️ **EXPORT_IMPORT_DESIGN.md**: Architecture and design decisions

---

**Version**: 1.0.0  
**Status**: Production Ready  
**Added**: 2025-12-19
