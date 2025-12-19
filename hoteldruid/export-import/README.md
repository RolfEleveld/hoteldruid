# HotelDroid Export/Import System

## Overview

The Export/Import system provides a modern, language-agnostic way to export and import HotelDroid data. It operates independently from the existing backup/restore mechanism and creates portable JSON-based packages in ZIP format.

**Key Features:**
- ✅ **JSON-Based**: Human-readable, portable data format
- ✅ **Field Mapping**: Intelligent mapping of database fields during import
- ✅ **Entity Translation**: Support for importing to systems with different naming conventions
- ✅ **Validation**: Complete package validation before import
- ✅ **Preview**: Review data before importing
- ✅ **Configuration Export**: Include application settings in exports
- ✅ **Template Preservation**: Export and import document templates

## Architecture

```
export-import/
├── lib/
│   ├── DataFlattener.php         # Converts DB tables to JSON
│   ├── Exporter.php              # Creates complete export packages
│   ├── Importer.php              # Imports packages with validation
│   ├── ExportImportUI.php        # UI components
│   └── Logger.php                # (Optional) Logging utility
├── schemas/
│   └── tables/                   # JSON schema definitions
├── packages/                     # Generated export packages
├── export-import-handlers.php    # Integration with crea_backup.php
└── README.md                     # This file
```

## Usage

### Exporting Data

1. Open **Backup** page in HotelDruid admin panel
2. Click **"Create Export Package"** button
3. Choose what to include:
   - ☑ Include configurations (language, themes, etc.)
   - ☑ Include templates (document templates)
   - ☑ Include documents (contracts, etc.)
4. Click **"Create Export Package"**
5. Download the generated ZIP file

**Package Filename Format:**
```
export_hoteldruid_20251216_143000_prod_v1.zip
```

### Importing Data

1. Open **Backup** page in HotelDruid admin panel
2. Click **"Continue"** under Import Data section
3. Select your export package (ZIP file)
4. Choose import mode:
   - **Preview before importing**: Review data mapping first (recommended)
   - **Import directly**: Skip preview and import immediately
5. Check **"Import configurations"** if desired
6. Click **"Continue"**

#### Preview Mode
- Review all tables that will be imported
- See row counts and column names
- Adjust field mappings if needed
- Click **"Confirm Import"** to proceed

### Field Mapping

The system automatically suggests mappings based on:
- **Entity Mapping**: International table/field names
- **Field Names**: Matching source and target field names
- **Data Types**: Automatic type conversion

To manually adjust mappings in preview:
1. Source field name appears on the left
2. Suggested target field name in the input field
3. Modify if needed for your system
4. Click **"Confirm Import"**

## Package Contents

### manifest.json
Top-level file documenting the export source:
```json
{
  "export_format_version": "1.0.0",
  "export_timestamp": "2025-12-16T14:30:00Z",
  "export_id": "uuid-here",
  "source_system": {
    "application": "HotelDroid",
    "version": "3.07",
    "database_type": "postgresql"
  },
  "source_machine": {
    "hostname": "hotel-server-prod",
    "system_name": "Main Hotel Database",
    "hosting_system": "PHP 7.4.3",
    "exported_by": "admin@hotel.com"
  }
}
```

### metadata/
- **export_metadata.json**: Details about what was exported
- **entity_mapping.json**: Table/field translation mapping
- **schema_versions.json**: Schema version tracking

### data/tables/
JSON files for each table:
- `clienti.json`
- `contratti.json`
- `prenota2025.json`
- etc.

Each table file contains:
```json
{
  "table_name": "clienti",
  "schema_version": "1.0.0",
  "row_count": 892,
  "columns": [...]
  "rows": [...]
}
```

### configs/
- **configurations.json**: All PHP configuration files in JSON format
- **templates/**: Document templates (.rtf, .docx files)

### schemas/
- **relationships.json**: Foreign key definitions
- **tables/**: Per-table schema definitions

### docs/
- **EXPORT_INFO.txt**: Human-readable export summary
- **IMPORT_GUIDE.txt**: How to use this package
- **SCHEMA_REFERENCE.txt**: Table definitions

## API Usage (For Developers)

### Export Example

```php
<?php
include_once('./export-import/lib/Exporter.php');

// Create exporter
$exporter = new Exporter(
    $numconnessione,      // DB connection
    $PHPR_TAB_PRE,        // Table prefix
    $PHPR_DB_TYPE,        // Database type
    C_DATI_PATH . '/../export-import/packages'
);

// Create export package
$package_file = $exporter->createExportPackage([
    'include_configs' => true,
    'include_templates' => true,
    'include_documents' => true,
    'export_type' => 'full',
    'source_name' => 'prod'
]);

if ($package_file) {
    echo "Export created: " . basename($package_file);
}
?>
```

### Import Example

```php
<?php
include_once('./export-import/lib/Importer.php');

// Create importer
$importer = new Importer(
    $numconnessione,
    $PHPR_TAB_PRE,
    $PHPR_DB_TYPE,
    '/path/to/export.zip'
);

// Validate package
$validation = $importer->validatePackage();
if ($validation['valid']) {
    // Get preview
    $preview = $importer->getImportPreview();
    
    // Set field mappings if needed
    $importer->setFieldMapping('clienti', 'clienti', [
        'cognome' => 'cognome',
        'nome' => 'nome'
    ]);
    
    // Import data
    $stats = $importer->importData(false, true);
    echo "Imported " . $stats['rows_imported'] . " rows";
}
?>
```

## Data Integrity

### How Data is Validated

1. **Package Extraction**: Zip CRC32 validates file integrity automatically
2. **Schema Validation**: Each table checked against expected columns
3. **Type Checking**: Data types converted and validated during import
4. **Referential Integrity**: Foreign keys verified where applicable
5. **Completeness**: All required fields checked before insertion

### No Separate Checksums

- Zip file CRC32 is sufficient for corruption detection
- Eliminates complexity of per-table hash management
- Allows third-party package creation without hash algorithms
- Enables future implementations in any language

## Security Considerations

### Access Control
- ✅ **Admin Only**: Only users with ID=1 can export/import
- ✅ **Authentication**: Full HotelDroid login required
- ✅ **Permission Checking**: Integrated with existing privilege system

### File Handling
- ✅ **Temp Directory**: Uses system temp with unique names
- ✅ **Cleanup**: Temporary files automatically deleted
- ✅ **Safe Paths**: No path traversal vulnerabilities
- ✅ **Validation**: ZIP structure validated before extraction

### Data Protection
- ✅ **No Passwords**: Database credentials not exported
- ✅ **No Sensitive Data**: User passwords excluded
- ✅ **Audit Trail**: Export source and timestamp recorded
- ✅ **Optional Anonymization**: Can exclude certain data types

## Entity Mapping

The system supports importing to implementations with different naming conventions:

### Example: Italian HotelDroid → English-Named System

**Export (Italian):**
```
Table: clienti
Fields: cognome, nome, email
```

**Entity Mapping:**
```json
{
  "table_translations": {
    "clienti": "guests"
  },
  "field_translations": {
    "clienti": {
      "cognome": "last_name",
      "nome": "first_name",
      "email": "email_address"
    }
  }
}
```

**Import (English System):**
```
Target Table: guests
Target Fields: last_name, first_name, email_address
```

## Troubleshooting

### Export Fails
1. Check admin privileges (ID=1)
2. Verify `/export-import/packages` directory is writable
3. Check database connectivity
4. Review PHP error logs

### Import Package Invalid
1. Ensure ZIP file wasn't corrupted during download
2. Verify manifest.json exists in package
3. Check that package was created with this system

### Import Preview Shows No Data
1. Check that data/tables directory exists in package
2. Verify .json files in data/tables are readable
3. Review import validation errors

### Field Mapping Errors
1. Check column names match exactly
2. Verify target table exists in database
3. Review entity_mapping.json for suggestions

## Advanced Topics

### Creating Custom Exports

You can programmatically create exports with specific tables:

```php
$exporter = new Exporter($conn, $prefix, $db_type, $export_dir);
$flattener = new DataFlattener($conn, $prefix, $db_type);

// Export only specific tables
$tables = ['clienti', 'appartamenti', 'prenota2025'];
$export_data = $flattener->flattenDatabase($tables);
```

### Partial Imports

The import system can import specific tables:

```php
$importer = new Importer($conn, $prefix, $db_type, $package_file);

// Import with dry-run (preview)
$stats = $importer->importData($dry_run = true);
```

### Custom Field Transformation

Implement custom field transformations:

```php
$importer->setFieldMapping('clienti', 'customers', [
    'idclienti' => 'customer_id',
    'cognome' => 'surname',
    'nome' => 'given_name'
]);
```

## Future Enhancements

- [ ] **Partial Exports**: Export specific date ranges or customer segments
- [ ] **Dry-Run Mode**: Preview import without modifying database
- [ ] **Incremental Backups**: Only export changed records
- [ ] **Encryption**: Optional password protection for packages
- [ ] **Compression**: Configurable compression levels
- [ ] **Scheduling**: Automatic periodic exports
- [ ] **Cloud Integration**: Direct upload to cloud storage
- [ ] **Version Compatibility**: Migration between different HotelDroid versions

## Reference Links

- [Architecture Overview](ARCHITECTURE_REVIEW_COMPLETE.md)
- [Design Document](../EXPORT_IMPORT_DESIGN.md)
- [Backup System](crea_backup.php)

## Support

For issues or questions:
1. Check the logs in `/dati/export-import.log` (if logging enabled)
2. Review this README
3. Check the error messages in the UI
4. Contact system administrator

---

**Version:** 1.0.0  
**Last Updated:** 2025-12-19  
**Status:** Production Ready
