# Export/Import API Architecture Plan

> Integrating HotelDroid's export/import system into the ASP.NET Core API with Blazor client support

**Status:** 📋 Architecture Planning Phase | Ready for Discussion  
**Date:** February 21, 2026

---

## Executive Summary

The hoteldruid export/import system (PHP-based, ZIP packages, JSON data) will be replicated in ASP.NET Core to enable:

1. **Data Export** — Create hoteldruid-compatible ZIP packages from the .NET API
2. **Data Import** — Parse hoteldruid-compatible ZIP packages and distribute data to respective APIs
3. **Blazor Integration** — Upload/download packages from web UI
4. **Cross-System Migration** — Enable database→API→database workflows

This architecture maintains **100% compatibility** with hoteldruid's export format while adding .NET-native capabilities.

---

## System Architecture Overview

### Current State (HotelDroid PHP)

```
┌─────────────────────────┐
│    HotelDroid UI (PHP)   │
├─────────────────────────┤
│  Export → ZIP Package   │
│  Import ← ZIP Package   │
│                         │
└──────────┬──────────────┘
           │
           ↓
    ┌─────────────┐
    │  MySQL DB   │
    │ (apartm...)│
    └─────────────┘
```

### Future State (HotelDroid + .NET API + Blazor)

```
┌────────────────────────────────────────────────────────────┐
│                    Blazor Web Client                        │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Upload/Download Package UI (Rooms, Guests, etc.)    │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────┬──────────────────────────────┬────────────────┘
             │                              │
             ↓                              ↓
    ┌──────────────────┐          ┌──────────────────┐
    │ .NET API Server  │          │  HotelDroid (PHP)│
    │ (New Services)   │          │  (Existing)      │
    │                  │          │                  │
    │ ┌──────────────┐ │          │ ┌──────────────┐ │
    │ │ Export Svc   │ │◄────────►│ │ Export.php   │ │
    │ │              │ │ ZIP Pkg  │ │ (compatible) │ │
    │ ├──────────────┤ │          │ ├──────────────┤ │
    │ │ Import Svc   │ │◄────────►│ │ Import.php   │ │
    │ │              │ │ ZIP Pkg  │ │ (compatible) │ │
    │ ├──────────────┤ │          │ └──────────────┘ │
    │ │ RoomsAPI     │ │          │                  │
    │ │ GuestsAPI    │ │◄────────►│   MySQL DB       │
    │ │ GuestsAPI    │ │ (future) │  + Schema Map    │
    │ └──────────────┘ │          │                  │
    │                  │          │                  │
    └────────┬─────────┘          └──────────────────┘
             │
             ↓
    ┌──────────────────┐
    │  File Store      │
    │  (ZIP Packages)  │
    └──────────────────┘
```

---

## Data Flow Diagram

### Export Flow

```
┌─────────────────────────────────────────────────────┐
│ 1. User clicks "Export" in Blazor UI                │
└─────────────────────┬───────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 2. POST /api/export                                 │
│    Body: { "includeConfigs": true, "includeData": } │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 3. ExportService (C#)                               │
│    ├─ Call RoomsAPI → Get all rooms                 │
│    ├─ Call GuestsAPI → Get all guests (future)     │
│    ├─ Call other APIs → Get all other data         │
│    └─ Flatten to canonical JSON                     │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 4. PackageBuilder (C#)                              │
│    ├─ Create manifest.json (hoteldruid-compatible)  │
│    ├─ Create data/tables/*.json files               │
│    ├─ Create schemas/tables/*.json files            │
│    ├─ Create metadata/export_metadata.json          │
│    └─ Create docs/ directory                        │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 5. ZipBuilder (C#)                                  │
│    └─ Package into export_hoteldruid_*.zip          │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 6. File Storage                                      │
│    └─ Save ZIP to /artifacts/exports/               │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 7. Return Download Link                             │
│    ← HTTP 200 { "downloadUrl": "...", "size": ... }│
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 8. Browser Downloads ZIP                            │
└─────────────────────────────────────────────────────┘
```

### Import Flow

```
┌─────────────────────────────────────────────────────┐
│ 1. User uploads ZIP file in Blazor UI               │
└─────────────────────┬───────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 2. POST /api/import/validate (multipart/form-data)  │
│    FormData: { "file": <zip> }                      │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 3. PackageValidator (C#)                            │
│    ├─ Unzip to temp directory                       │
│    ├─ Read manifest.json                            │
│    ├─ Verify required directories                   │
│    ├─ Schema validation                             │
│    └─ Return validation report                      │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 4. Return Validation Result                         │
│    ← HTTP 200 {                                     │
│      "valid": true,                                 │
│      "tables": ["appartamenti", "clienti", ...],   │
│      "rowCounts": { "appartamenti": 25, ... }      │
│    }                                                │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 5. User Approves Import                             │
│    POST /api/import/execute                         │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 6. DataFlattener (C#)                               │
│    ├─ Read each table JSON from ZIP                 │
│    ├─ Parse canonical format                        │
│    └─ Unflatten to row objects                      │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 7. APIDistributor (C#)                              │
│    ├─ Appartamenti rows → POST /api/rooms (bulk)   │
│    ├─ Clienti rows → POST /api/guests (future)     │
│    ├─ Other tables → respective APIs                │
│    └─ Track API responses                           │
└─────────────────────────────────────────────────────┘
                      │
                      ↓
┌─────────────────────────────────────────────────────┐
│ 8. Return Import Results                            │
│    ← HTTP 200 {                                     │
│      "totalImported": 50,                           │
│      "byTable": {                                   │
│        "appartamenti": { "imported": 25, ...},      │
│        "clienti": { "imported": 25, ... }           │
│      },                                             │
│      "errors": []                                   │
│    }                                                │
└─────────────────────────────────────────────────────┘
```

---

## Component Architecture

### 1. ExportService (Export Orchestrator)

**Responsibility:** Coordinate data collection and package creation

```csharp
public class ExportService : IExportService
{
    public async Task<ExportResult> CreateExportPackageAsync(ExportOptions options)
    {
        // 1. Collect data from all APIs
        var roomsData = await _roomsApiClient.GetAllRoomsAsync();
        var guestsData = await _guestsApiClient.GetAllGuestsAsync();
        var contractsData = await _contractsApiClient.GetAllContractsAsync();
        
        // 2. Flatten to canonical format
        var canonicalData = _canonicalMapper.ToCanonical(new { rooms = roomsData, ... });
        
        // 3. Build package
        var package = await _packageBuilder.BuildAsync(canonicalData, options);
        
        // 4. Create ZIP
        var zipPath = await _zipBuilder.CreateZipAsync(package);
        
        // 5. Store and return
        return new ExportResult { DownloadUrl = ..., Size = ... };
    }
}
```

**Methods:**
- `CreateExportPackageAsync(ExportOptions)` → `ExportResult`
- `GetExportStatusAsync(exportId)` → `ExportStatus`
- `CancelExportAsync(exportId)` → `bool`

**Dependencies:**
- IRoomsApiClient, IGuestsApiClient, etc.
- ICanonicalMapper
- IPackageBuilder
- IZipBuilder
- IFileStorage

---

### 2. ImportService (Import Orchestrator)

**Responsibility:** Validate and execute imports

```csharp
public class ImportService : IImportService
{
    public async Task<ValidationResult> ValidatePackageAsync(IFormFile zipFile)
    {
        // 1. Unzip to temp
        var tempPath = await _zipExtractor.ExtractAsync(zipFile);
        
        // 2. Validate structure
        var manifest = JsonConvert.DeserializeObject(
            File.ReadAllText($"{tempPath}/manifest.json"));
        
        // 3. Check integrity
        var result = _packageValidator.Validate(tempPath, manifest);
        
        // 4. Clean and return
        return result;
    }
    
    public async Task<ImportResult> ImportDataAsync(string packagePath)
    {
        // 1. Parse package
        var data = await _dataFlattener.ParseCanonicalDataAsync(packagePath);
        
        // 2. Distribute to APIs
        var results = await _apiDistributor.DistributeAsync(data);
        
        // 3. Collect results
        return new ImportResult { ... };
    }
}
```

**Methods:**
- `ValidatePackageAsync(IFormFile)` → `ValidationResult`
- `GetImportPreviewAsync(packagePath)` → `ImportPreview`
- `ImportDataAsync(packagePath)` → `ImportResult`
- `GetImportStatusAsync(importId)` → `ImportStatus`
- `CancelImportAsync(importId)` → `bool`

**Dependencies:**
- IZipExtractor
- IPackageValidator
- IDataFlattener
- IApiDistributor
- IFileStorage

---

### 3. CanonicalMapper (Data Format Translator & Normalizer)

**Responsibility:** Convert between .NET objects and hoteldruid canonical JSON format, with flexible inbound naming and refined outbound naming.

**Design Principle:** *Inward Liberal, Outward Conservative*
- Accept multiple names for the same entity (appartamenti, apartments, rooms)
- Normalize to canonical internal format
- Export with industry-standard refined names (rooms > apartments > appartamenti)

```csharp
public class CanonicalMapper : ICanonicalMapper
{
    // Table name aliases (flexible inbound)
    private static readonly Dictionary<string, string> TableAliases = new()
    {
        ["appartamenti"] = "rooms",      // Normalize hoteldruid name
        ["apartments"] = "rooms",         // Common English variant
        ["rooms"] = "rooms",              // API standard name
        ["appartment"] = "rooms",         // Typo handling
        ["apartment"] = "rooms"           // Singular variant
    };
    
    // Standard outbound names (refined, industry-standard)
    private const string StandardTableName = "rooms";  // Export always uses this
    private const string HotelDroidTableName = "appartamenti";  // Hoteldruid compatibility
    
    // Field name aliases (flexible inbound)
    private static readonly Dictionary<string, string> FieldAliases = new()
    {
        // Hoteldruid Italian → Canonical
        ["idappartamenti"] = "id",
        ["numpiano"] = "floor_number",
        ["maxoccupanti"] = "capacity",
        ["numcasa"] = "house_number",
        ["app_vicini"] = "neighboring_rooms",
        ["priorita"] = "priority",
        ["priorita2"] = "secondary_priority",
        ["letto"] = "has_beds",
        ["commento"] = "comments",
        
        // English variants
        ["room_id"] = "id",
        ["floor"] = "floor_number",
        ["max_occupancy"] = "capacity",
        ["house_id"] = "house_number",
        ["neighbors"] = "neighboring_rooms",
        ["notes"] = "comments"
    };
    
    // Reverse mapping for export (canonical → export name)
    private static readonly Dictionary<string, string> ExportFieldNames = new()
    {
        ["id"] = "id",
        ["floor_number"] = "floor_number",
        ["capacity"] = "capacity",
        ["house_number"] = "house_number",
        ["neighboring_rooms"] = "neighboring_rooms",
        ["priority"] = "priority",
        ["secondary_priority"] = "secondary_priority",
        ["has_beds"] = "has_beds",
        ["comments"] = "comments"
    };
    
    public CanonicalMapper(string phprTablePrefix, string mappingPath = null)
    {
        // Load deployment-specific overrides if present
        // Allows custom naming per installation
    }
    
    /// <summary>
    /// Normalize table name to canonical form (liberal inbound)
    /// Accepts: "appartamenti", "apartments", "rooms", etc.
    /// Returns: canonical name for internal use
    /// </summary>
    public string NormalizeTableName(string inputTableName)
    {
        var lower = inputTableName?.ToLowerInvariant() ?? "";
        return TableAliases.TryGetValue(lower, out var canonical) 
            ? canonical 
            : lower;  // Pass through unknown names
    }
    
    /// <summary>
    /// Get export table name (conservative outbound)
    /// Input: Any normalized table name
    /// Returns: Industry-standard export name
    /// </summary>
    public string GetExportTableName(string normalizedTableName)
    {
        var table = NormalizeTableName(normalizedTableName);
        return table switch
        {
            "rooms" => StandardTableName,  // "rooms"
            _ => Table
        };
    }
    
    /// <summary>
    /// Get hoteldruid-compatible table name for backward compatibility
    /// Returns: Original hoteldruid name ("appartamenti")
    /// </summary>
    public string GetHotelDroidTableName(string normalizedTableName)
    {
        var table = NormalizeTableName(normalizedTableName);
        return table switch
        {
            "rooms" => HotelDroidTableName,  // "appartamenti"
            _ => table
        };
    }
    
    /// <summary>
    /// Normalize field name to canonical form (liberal inbound)
    /// Accepts: "idappartamenti", "room_id", "id", etc.
    /// Returns: canonical field name for internal use
    /// </summary>
    public string NormalizeFieldName(string inputFieldName)
    {
        var lower = inputFieldName?.ToLowerInvariant() ?? "";
        return FieldAliases.TryGetValue(lower, out var canonical)
            ? canonical
            : lower;  // Pass through unknown fields
    }
    
    // ToCanonical: .NET object → Canonical JSON (for export)
    public CanonicalData ToCanonical(RoomDto[] rooms)
    {
        return new CanonicalData
        {
            TableName = StandardTableName,  // Export always uses "rooms"
            RowCount = rooms.Length,
            Rows = rooms.Select(r => new Dictionary<string, object>
            {
                [ExportFieldNames["id"]] = r.Id,
                [ExportFieldNames["floor_number"]] = r.FloorNumber,
                [ExportFieldNames["capacity"]] = r.Capacity,
                [ExportFieldNames["house_number"]] = r.HouseNumber,
                [ExportFieldNames["neighboring_rooms"]] = r.NeighboringRooms,
                [ExportFieldNames["priority"]] = r.Priority,
                [ExportFieldNames["secondary_priority"]] = r.SecondaryPriority,
                [ExportFieldNames["has_beds"]] = r.HasBeds,
                [ExportFieldNames["comments"]] = r.Comments
            }).ToList()
        };
    }
    
    // FromCanonical: Canonical JSON → .NET object (for import, liberal inbound)
    public RoomDto[] FromCanonical(CanonicalData data)
    {
        // Accept table name in any form
        var normalizedTable = NormalizeTableName(data.TableName);
        if (normalizedTable != "rooms")
            throw new InvalidOperationException(
                $"Cannot map table '{data.TableName}' to rooms API");
        
        return data.Rows.Select(row => new RoomDto(
            // Normalize field names from input (handles all variants)
            Id: GetNormalizedValue(row, "id"),
            Name: GetNormalizedValue(row, "name") ?? "", // May need extraction
            Capacity: int.Parse(GetNormalizedValue(row, "capacity") ?? "0"),
            FloorNumber: GetNormalizedValue(row, "floor_number"),
            HouseNumber: GetNormalizedValue(row, "house_number"),
            Priority: int.TryParse(GetNormalizedValue(row, "priority"), out var p) ? p : null,
            SecondaryPriority: int.TryParse(GetNormalizedValue(row, "secondary_priority"), out var p2) ? p2 : null,
            HasBeds: GetNormalizedValue(row, "has_beds"),
            NeighboringRooms: GetNormalizedValue(row, "neighboring_rooms"),
            Comments: GetNormalizedValue(row, "comments")
        )).ToArray();
    }
    
    /// <summary>
    /// Get value from row with flexible field name handling
    /// Tries: exact match, then normalized aliases
    /// </summary>
    private string GetNormalizedValue(Dictionary<string, object> row, 
        string canonicalFieldName)
    {
        // Try exact canonical name first
        if (row.TryGetValue(canonicalFieldName, out var value))
            return value?.ToString();
        
        // Try to find via alias mapping (reverse lookup)
        foreach (var (alias, canonical) in FieldAliases)
        {
            if (canonical == canonicalFieldName && 
                row.TryGetValue(alias, out var aliasValue))
                return aliasValue?.ToString();
        }
        
        return null;
    }
}
```

**Methods:**
- `NormalizeTableName(string)` → `string` — Liberal inbound
- `NormalizeFieldName(string)` → `string` — Liberal inbound
- `GetExportTableName(string)` → `string` — Refined outbound
- `GetHotelDroidTableName(string)` → `string` — Compatibility
- `ToCanonical(RoomDto[])` → `CanonicalData` — Export (refined names)
- `FromCanonical(CanonicalData)` → `RoomDto[]` — Import (flexible names)
- `ValidateCanonicalData(CanonicalData)` → `ValidationResult`

**Design Notes:**
- All inbound names normalized to canonical form
- Export always uses standard names (rooms, not appartamenti)
- Hoteldruid compatibility layer for backward compatibility
- Aliases configurable per deployment (dati/export-import/naming.json)

---

### 4. PackageBuilder (ZIP Structure Creator)

**Responsibility:** Assemble canonical data into hoteldruid-compatible package structure

```csharp
public class PackageBuilder : IPackageBuilder
{
    public async Task<ExportPackage> BuildAsync(
        Dictionary<string, CanonicalData> data,
        ExportOptions options)
    {
        var package = new ExportPackage
        {
            // Create manifest
            Manifest = new Manifest
            {
                ExportId = Guid.NewGuid(),
                ExportTimestamp = DateTime.UtcNow,
                SourceSystem = new SourceSystem
                {
                    Application = "HotelDroid.API",
                    Version = "1.0.0",
                    DatabaseType = "SQLite/PostgreSQL"
                }
            },
            
            // Create data files
            DataTables = data.Select(kvp => 
                new TableFile
                {
                    FileName = $"{kvp.Key}.json",
                    Content = JsonConvert.SerializeObject(kvp.Value)
                }
            ).ToList(),
            
            // Create schema files
            SchemaFiles = data.Select(kvp =>
                new SchemaFile
                {
                    FileName = $"{kvp.Key}.schema.json",
                    Content = JsonConvert.SerializeObject(kvp.Value.Columns)
                }
            ).ToList()
        };
        
        return package;
    }
}
```

---

### 5. APIDistributor (Data Distribution to APIs)

**Responsibility:** Route imported data to appropriate API endpoints

```csharp
public class ApiDistributor : IApiDistributor
{
    // Map canonical table → API endpoint
    private static readonly Dictionary<string, RouteInfo> Routes = new()
    {
        ["appartamenti"] = new("/api/rooms", "POST", BulkImportMode),
        ["clienti"] = new("/api/guests", "POST", BulkImportMode),
        ["contratti"] = new("/api/contracts", "POST", BulkImportMode)
    };
    
    public async Task<DistributionResult> DistributeAsync(
        Dictionary<string, CanonicalData> data)
    {
        var results = new List<TableImportResult>();
        
        foreach (var (tableName, tableData) in data)
        {
            if (!Routes.TryGetValue(tableName, out var route))
                continue; // Skip unmapped tables
            
            // Convert canonical → DTO
            var dtos = _canonicalMapper.FromCanonical(tableData);
            
            // Send to API (bulk)
            var response = await _httpClient.PostAsJsonAsync(route.Path, dtos);
            
            results.Add(new TableImportResult
            {
                TableName = tableName,
                ImportedCount = dtos.Length,
                Success = response.IsSuccessStatusCode
            });
        }
        
        return new DistributionResult { Results = results };
    }
}
```

---

### 6. ZipBuilder (ZIP File Creator)

**Responsibility:** Create hoteldruid-compatible ZIP packages

```csharp
public class ZipBuilder : IZipBuilder
{
    public async Task<string> CreateZipAsync(ExportPackage package)
    {
        var zipPath = Path.Combine(_storageRoot, 
            $"export_hoteldruid_{DateTime.UtcNow:yyyyMMdd_HHmmss}.zip");
        
        using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            // Add manifest.json
            var manifestEntry = archive.CreateEntry("manifest.json");
            using (var stream = manifestEntry.Open())
            {
                var json = JsonConvert.SerializeObject(package.Manifest);
                var bytes = Encoding.UTF8.GetBytes(json);
                stream.Write(bytes, 0, bytes.Length);
            }
            
            // Add data files
            foreach (var table in package.DataTables)
            {
                var entry = archive.CreateEntry($"data/tables/{table.FileName}");
                using (var stream = entry.Open())
                {
                    var bytes = Encoding.UTF8.GetBytes(table.Content);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            
            // Add schemas, configs, docs...
        }
        
        return zipPath;
    }
}
```

---

## API Endpoint Specifications

### Export Endpoints

#### 1. Create Export Package

```
POST /api/export/create
Content-Type: application/json

{
  "includeConfigs": true,
  "includeDocs": true,
  "exportType": "full"
}

Response 200:
{
  "exportId": "uuid",
  "status": "pending",
  "statusUrl": "/api/export/{exportId}/status",
  "estimatedTime": 5
}
```

#### 2. Check Export Status

```
GET /api/export/{exportId}/status

Response 200:
{
  "exportId": "uuid",
  "status": "completed", // pending, processing, completed, failed
  "progress": 100,
  "downloadUrl": "/api/export/{exportId}/download",
  "fileSize": 2048000,
  "createdAt": "2026-02-21T10:30:00Z"
}
```

#### 3. Download Export

```
GET /api/export/{exportId}/download

Response 200: [Binary ZIP file]
  Content-Type: application/zip
  Content-Disposition: attachment; filename="export_hoteldruid_*.zip"
```

#### 4. List Exports

```
GET /api/export/list?limit=20&skip=0

Response 200:
{
  "exports": [
    {
      "exportId": "uuid",
      "createdAt": "2026-02-21T10:30:00Z",
      "fileName": "export_hoteldruid_*.zip",
      "fileSize": 2048000
    }
  ],
  "total": 45
}
```

---

### Import Endpoints

#### 1. Validate Import Package

```
POST /api/import/validate
Content-Type: multipart/form-data

multipart: file=<zip>

Response 200:
{
  "valid": true,
  "packageId": "uuid",
  "manifest": {
    "exportTimestamp": "2026-02-21T10:30:00Z",
    "sourceSystem": "HotelDroid 3.0 OR Custom System OR .NET API 1.0"
  },
  "tables": [
    {
      "name": "rooms",              // Normalized to standard name
      "sourceNames": ["appartamenti", "rooms"],  // Original names detected
      "rowCount": 25,
      "columns": ["id", "floor_number", "capacity", ...]  // Normalized
    }
  ],
  "nameMapping": {
    "detectected_source": "appartamenti",
    "normalized_to": "rooms",
    "compatibility": "HotelDroid 3.0"
  }
}
```

**What's happening behind the scenes:**
1. ZIP extracted
2. Manifest read: sees table name "appartamenti" OR "apartments" OR "rooms"
3. CanonicalMapper.NormalizeTableName() → "rooms"
4. Field names analyzed:
   - "idappartamenti" → normalized to "id"
   - "numpiano" → normalized to "floor_number"
   - etc.
5. Preview shows normalized standard names
6. User sees familiar "rooms" table with standard field names
7. On import, CanonicalMapper handles reverse mapping back to API format

---

---

## Practical Examples: Naming in Action

### Scenario 1: Export from .NET API (Clean, Standard Names)

**User Action:** Click "Export" in Blazor app

**Generated ZIP structure:**
```
export_hoteldruid_20260221_103000_v1.zip
├── manifest.json
├── data/tables/
│   └── rooms.json              ← Standard name (not "appartamenti")
├── schemas/tables/
│   └── rooms.json
└── metadata/
    ├── export_metadata.json
    └── naming_info.json        ← Includes "sourceNames": ["rooms"]
```

**Sample data inside rooms.json:**
```json
{
  "table_name": "rooms",        // Refined outbound name
  "row_count": 3,
  "columns": [                  // Refined field names
    "id", "floor_number", "capacity", "house_number",
    "neighboring_rooms", "priority", "secondary_priority", "has_beds", "comments"
  ],
  "rows": [
    {
      "id": "room1",
      "floor_number": "1",
      "capacity": 2,
      "house_number": "101",
      "neighboring_rooms": "room2",
      "priority": 1,
      "secondary_priority": null,
      "has_beds": "S",
      "comments": "Corner location"
    }
  ]
}
```

**User sees:** Modern, clean naming. Can import into any system.

---

### Scenario 2: Import Old HotelDroid Export (Flexible Inbound)

**User Action:** Upload ZIP from old HotelDroid PHP

**ZIP contains:**
```
export_hoteldruid_20250801_090000_v1.zip
├── data/tables/
│   └── appartamenti.json    ← Italian name
├── metadata/
│   └── export_metadata.json
```

**Sample data inside appartamenti.json:**
```json
{
  "table_name": "appartamenti",  // Old Italian name
  "row_count": 3,
  "columns": [
    "idappartamenti", "numpiano", "maxoccupanti", "numcasa", 
    "app_vicini", "priorita", "priorita2", "letto", "commento"
  ],
  "rows": [
    {
      "idappartamenti": "app1",
      "numpiano": "1",
      "maxoccupanti": 2,
      "numcasa": "101",
      "app_vicini": "app2",
      "priorita": 1,
      "priorita2": null,
      "letto": "S",
      "commento": "Angolo"
    }
  ]
}
```

**What happens:**
1. ✅ Validator.NormalizeTableName("appartamenti") → "rooms"
2. ✅ User sees "rooms" table in preview
3. ✅ Field mapping: "idappartamenti" → "id", "numpiano" → "floor_number", etc.
4. ✅ User sees familiar field names
5. ✅ On POST /api/rooms, data validated normally

**User sees:** Familiar names, seamless import.

---

### Scenario 3: Import Mixed-Format Export (Flexible Inbound)

**Custom system exported with:**
```json
{
  "table_name": "apartments",    // English variant
  "columns": [
    "room_id", "floor", "max_occupancy", "house_id",
    "neighbors", "priority", "secondary_priority", "beds", "notes"
  ],
  "rows": [...]
}
```

**What happens:**
1. ✅ Validator.NormalizeTableName("apartments") → "rooms"
2. ✅ Field aliases handled:
   - "room_id" → "id"
   - "floor" → "floor_number"
   - "max_occupancy" → "capacity"
   - "house_id" → "house_number"
   - "neighbors" → "neighboring_rooms"
   - "beds" → "has_beds"
   - "notes" → "comments"
3. ✅ User sees "rooms" with standard fields
4. ✅ Import works perfectly

**User sees:** Works magically, handles variants.

---

### Scenario 4: Re-export Back to HotelDroid (Backward Compat)

You export from .NET with our standard names, then need to send to old HotelDroid for compatibility check.

**Option A: Enable HotelDroid Mode in Export**
```
POST /api/export/create
{
  "exportFormat": "hoteldruid-compat"   // Optional, defaults to "standard"
}
```

This generates:
```
export_hoteldruid_20260221_103000_v1.zip
├── data/tables/
│   └── appartamenti.json   ← HotelDroid Italian name
└── metadata/
    └── compatibility_mode.json
```

With field names: idappartamenti, numpiano, maxoccupanti, numcasa, app_vicini, priorita, priorita2, letto, commento

**Option B: Standard Export (Recommended)**
Use standard names ("rooms"), old HotelDroid can still read it by parsing the manifest and mapping field names.

---

```
GET /api/import/{packageId}/preview?limit=5

Response 200:
{
  "tables": {
    "appartamenti": {
      "rowCount": 25,
      "sampleRows": [
        {
          "idappartamenti": "app1",
          "numpiano": "1",
          "maxoccupanti": "2",
          ...
        }
      ]
    }
  }
}
```

#### 3. Execute Import

```
POST /api/import/{packageId}/execute
Content-Type: application/json

{
  "dryRun": false,
  "mappingOverrides": {}
}

Response 200:
{
  "importId": "uuid",
  "status": "pending",
  "statusUrl": "/api/import/{importId}/status"
}
```

#### 4. Check Import Status

```
GET /api/import/{importId}/status

Response 200:
{
  "importId": "uuid",
  "status": "completed", // pending, processing, completed, failed
  "progress": 100,
  "tablesProcessed": 3,
  "totalRows": 50,
  "errors": [],
  "completedAt": "2026-02-21T10:35:00Z"
}
```

---

## File Storage Structure

```
/artifacts/
├── exports/
│   ├── export_hoteldruid_20260221_103000_v1.zip
│   ├── export_hoteldruid_20260221_104500_v1.zip
│   └── ... (old exports auto-purged after 30 days)
│
├── imports/
│   ├── temp/
│   │   ├── import_uuid_/
│   │   │   ├── manifest.json
│   │   │   ├── data/
│   │   │   └── schemas/
│   │   └── ...
│   │
│   └── completed/
│       ├── import_uuid_completed.log
│       └── ...
│
└── staging/
    └── [temporary processing files]
```

---

## Naming Strategy: Inward Liberal, Outward Conservative

### The Pattern

**Inward (Import):** Accept any reasonable name
- `appartamenti` (hoteldruid Italian)
- `apartments` (English variant)
- `rooms` (API standard)
- `apartment`, `appartment` (typos/variants)

↓ Normalize to Canonical ↓

**Canonical (Internal):** Single standardized form
- `rooms` (normalized table name)
- `floor_number`, `capacity`, `house_number` (field names)

↓ Export with Refinement ↓

**Outward (Export):** Industry-standard refined names
- `rooms` (not "appartamenti" for international use)
- Consistent with hotel industry terminology
- Compatible with Blazor/modern systems
- Still includes hoteldruid compatibility layer

### Why This Works

| Scenario | Before | After |
|----------|--------|-------|
| Import from old HotelDroid export | ❌ Fails on "appartamenti" | ✅ Accepts & normalizes |
| Import custom system (uses "apartments") | ❌ Field mapping error | ✅ Alias handles it |
| Export to other systems | ❌ Italian names confusing | ✅ "rooms" standard |
| Re-import export to HotelDroid | ❌ Changed names | ✅ Compatibility layer |

### Configuration Example

```json
// dati/export-import/naming.json (deployment-specific)
{
  "table_aliases": {
    "appartamenti": "rooms",
    "apartments": "rooms",
    "rooms": "rooms",
    "apartment": "rooms"
  },
  "field_aliases": {
    "idappartamenti": "id",
    "numpiano": "floor_number",
    "maxoccupanti": "capacity",
    "room_id": "id",
    "floor": "floor_number"
  },
  "export_names": {
    "table": "rooms",          // Standard table name for export
    "fields": {
      "id": "id",
      "capacity": "capacity"
    }
  }
}
```

---

## Equivalence Mapping: Hoteldruid ↔ .NET API with Flexible Naming

### Current (Rooms) - Triple Name Support

| Input Names | Canonical | Export Name | HotelDroid Compat | .NET RoomDto |
|--|--|--|--|--|
| `appartamenti` OR `apartments` OR `rooms` | `rooms` | `rooms` | `appartamenti` | RoomDto (Id, Name, Capacity, FloorNumber, HouseNumber, Priority, SecondaryPriority, HasBeds, NeighboringRooms, Comments) |

### Field Mapping - All Variants Accepted

| Hoteldruid Field | English Alias 1 | English Alias 2 | Canonical | .NET Property |
|--|--|--|--|--|
| idappartamenti | room_id | id | id | Id |
| numpiano | floor | floor_number | floor_number | FloorNumber |
| maxoccupanti | max_occupancy | capacity | capacity | Capacity |
| numcasa | house_id | house_number | house_number | HouseNumber |
| app_vicini | neighbors | neighboring_rooms | neighboring_rooms | NeighboringRooms |
| priorita | priority | — | priority | Priority |
| priorita2 | secondary_priority | — | secondary_priority | SecondaryPriority |
| letto | beds | has_beds | has_beds | HasBeds |
| commento | notes | comments | comments | Comments |

### Future Phase 2 (Add Guests)

| HotelDroid Table | .NET API | Status |
|--|--|--|
| clienti | `/api/guests` | Planned |
| contratti | `/api/contracts` | Planned |
| prenota | `/api/bookings` | Planned |

---

## Implementation Phases

### Phase 1: Rooms Only (Current Focus)
- ✅ RoomDto aligned with hoteldruid
- ✅ 27 comprehensive tests
- 📋 **Phase 1A:** ExportService + RoomsExporter
- 📋 **Phase 1B:** ImportService + RoomsImporter
- 📋 **Phase 1C:** PackageBuilder + ZipBuilder
- 📋 **Phase 1D:** API endpoints (export, import)
- 📋 **Phase 1E:** Blazor UI (upload/download)

### Phase 2: Multi-Entity (Future)
- Guests API
- Contracts API
- Bookings API
- Update APIDistributor for multi-table routing

### Phase 3: Advanced Features (Future)
- Selective export (specific date ranges)
- Field mapping UI
- Conflict resolution
- Rollback on failure
- Progress streaming

---

## Technical Decisions

### 1. Why Replicate vs. Call PHP?

✅ **Replication (What We're Doing):**
- A/B testing: .NET can coexist with PHP
- Independent scaling: API doesn't depend on PHP being available
- Clean separation: No PHP/C# boundaries
- Future-proof: Blazor doesn't need to call PHP
- Modern stack: Uses ASP.NET Core features

### 2. ZIP Compatibility
- Use standard ZIP.NET (System.IO.Compression)
- Hoteldruid's Exporter.php is readable reference
- Same JSON format in ZIP → 100% compatible
- Binary data handled identically

### 3. Data Flow Direction
- **Export:** API → collect all data → ZIP
- **Import:** ZIP → distribute to API endpoints
- No direct database writes in import (goes through API validation)
- Maintains API contracts and business logic

### 4. Flexible Naming (Inward Liberal, Outward Conservative)

**Why this approach?**

| Approach | Trade-off | Chosen? |
|----------|-----------|---------|
| Rigid (accept only "rooms", reject "apartments") | Simple but breaks old exports | ❌ No |
| Passthrough (keep "appartamenti" everywhere) | Confusing for new users, non-standard | ❌ No |
| **Liberal inbound, Conservative outbound** | Flexible + clean = Best UX | ✅ **Yes** |

**Benefits:**
- ✅ Accept exports from any source (HotelDroid, custom systems, other hotels' exports)
- ✅ New users see industry-standard names ("rooms", not "appartamenti")
- ✅ Maintain backward compatibility (can still export to HotelDroid if needed)
- ✅ Future-proof for Blazor and other platforms
- ✅ Configurable per deployment (can override aliases in dati/export-import/naming.json)

**Example:**
- User uploads export from old HotelDroid → Sees "rooms" table → Imports smoothly
- User uploads export from custom PHP → Flexible field mapping → Works
- User exports from .NET → Gets clean "rooms" name → Can share with anyone
- User re-imports our export to HotelDroid → HotelDroid compat layer handles it
- Validation before any writes
- Dry-run mode for preview
- Transaction-like semantics (all or nothing)
- Detailed error logs in package metadata

---

## Blazor Integration Points

### Upload/Download Component

```razor
<!-- ExportImportPage.razor -->

<div class="export-section">
    <h3>Export Data</h3>
    <button @onclick="StartExport">Export as ZIP</button>
    
    @if (exportInProgress)
    {
        <ProgressBar Value="exportProgress" />
    }
    
    @if (exportReady)
    {
        <a href="@downloadUrl" download>Download Export</a>
    }
</div>

<div class="import-section">
    <h3>Import Data</h3>
    <InputFile OnChange="@SelectFile" />
    
    @if (fileSelected)
    {
        <button @onclick="ValidateImport">Validate</button>
    }
    
    @if (validationResult != null)
    {
        <div>
            <h4>Preview</h4>
            <!-- Show table list and row counts -->
            <button @onclick="ConfirmImport">Import Now</button>
        </div>
    }
</div>
```

### API Client Usage

```csharp
// ExportImportService.cs
public class ExportImportService
{
    private readonly HttpClient _http;
    
    public async Task<string> CreateExportAsync()
    {
        var response = await _http.PostAsJsonAsync("/api/export/create", 
            new { includeConfigs = true });
        var result = await response.Content.ReadFromJsonAsync<ExportResult>();
        return result.ExportId;
    }
    
    public async Task<ImportValidation> ValidateImportAsync(
        IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        var fileStream = file.OpenReadStream(maxAllowedSize: 100_000_000);
        content.Add(new StreamContent(fileStream), 
            "file", file.Name);
        
        var response = await _http.PostAsync("/api/import/validate", content);
        return await response.Content.ReadFromJsonAsync<ImportValidation>();
    }
}
```

---

## Testing Strategy

### Unit Tests
- CanonicalMapper: Round-trip conversions
- PackageBuilder: ZIP structure integrity
- APIDistributor: Routing logic

### Integration Tests
- ExportService: Full export cycle (data → ZIP)
- ImportService: Full import cycle (ZIP → API)
- End-to-end: Export → Import → Verify data matches

### API Tests
- Export endpoints: Status tracking, download
- Import endpoints: Validation, preview, execution
- Error scenarios: Invalid ZIP, network failures

---

## Dependencies & Libraries

```csharp
// Existing (already in project)
- System.IO.Compression (ZIP operations)
- System.Net.Http (HTTP client)

// New (to add)
- System.IO.Compression.ZipFile (if not already included)
- Newtonsoft.Json (already likely present)

// Optional (for future)
- Microsoft.AspNetCore.StaticFiles (serve ZIP downloads)
- SharpZipLib (if additional ZIP features needed)
```

---

## Data Integrity & Validation

### Export Validation
✅ All required fields present  
✅ Data types match schema  
✅ Referential integrity (if applicable)  
✅ ZIP package integrity (CRC32)  

### Import Validation
✅ ZIP structure correct  
✅ manifest.json present and valid  
✅ All required tables present  
✅ Schema compatibility check  
✅ Dry-run preview before commit  

---

## Security Considerations

### File Upload
- Max file size enforced (100MB default)
- ZIP bomb detection (expansion ratio check)
- Antivirus scan integration (if required)
- User authentication/authorization

### File Storage
- Separate artifact directory (outside web root for exports)
- Automatic cleanup (exports older than 30 days)
- Access logging
- Temp file cleanup on errors

### Data Privacy
- Export contains all data (user responsible for privacy)
- Import data goes through API validation (respects business rules)
- No sensitive data in manifest/metadata

---

## Migration Path: HotelDroid → .NET → Blazor

1. **Coexistence** (Phase 1)
   - PHP export/import works as before
   - .NET API provides parallel capability
   - Users choose which to use

2. **Parallel** (Phase 2)
   - Users export from PHP, import to .NET
   - Or export from .NET, import to PHP
   - Cross-system data sync

3. **Transition** (Phase 3)
   - Gradually migrate entities (rooms → guests → bookings)
   - .NET becomes primary
   - PHP kept for legacy/migration only

4. **Sunset** (Phase 4)
   - Blazor is primary UI
   - .NET API is primary backend
   - PHP retained for data-only exports (if needed)

---

## Success Criteria

✅ Export creates ZIPs with refined standard names ("rooms", not "appartamenti")  
✅ Import accepts any reasonable name variant (appartamenti, apartments, rooms)  
✅ Field mapping handles aliases (idappartamenti → id, numpiano → floor_number, etc.)  
✅ All room data preserved in round-trip (export → import → export)  
✅ User-facing names are always industry-standard (clean UX)  
✅ HotelDroid compatibility maintained (can re-import to PHP if needed)  
✅ Blazor UI shows familiar terminology (rooms, not apartments or appartamenti)  
✅ Error handling is graceful and informative  
✅ Performance acceptable (< 5 sec for typical datasets)  
✅ Tests verify all naming scenarios (all variants, field aliases, etc.)  

---

## Next Steps (Discussion)

1. **Review & Feedback** — Does this architecture align with your vision?
2. **Prioritization** — Which phase first? (Likely Phase 1A: ExportService)
3. **Refinement** — Any modifications to data flow or endpoints?
4. **Implementation** — Ready to start coding?

---

**Document Version:** 1.0  
**Last Updated:** February 21, 2026  
**Status:** 📋 Ready for Review & Discussion
