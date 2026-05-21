# Test Infrastructure Setup Guide

## Quick Start

### Run All Tests
```powershell
cd c:\Users\rolfe\git\own\Villa-Annunziata\HotelDruid
dotnet test
```

### Run Specific Test Suite
```powershell
# Unit tests only
dotnet test --filter "ClassName=FileKeyValueStoreTests"

# File system validation tests
dotnet test --filter "ClassName=FileKeyValueStoreFileSystemTests"

# API integration tests
dotnet test --filter "ClassName=RoomsApiTests"

# Ledger tests
dotnet test --filter "ClassName=LedgerApiTests"
```

### Use Test Runner Script
```powershell
cd scripts
.\run-tests.ps1

# With specific filter
.\run-tests.ps1 -Filter "FileKeyValueStore"

# In Release configuration
.\run-tests.ps1 -Configuration Release
```

---

## What's Been Implemented

### 1. FileKeyValueStore Tests
**Location**: `tests/HotelDruid.Api.Tests/Services/FileKeyValueStoreTests.cs`

Tests the core storage layer:
- ID generation (GUID-base32)
- CRUD operations
- Index management
- Concurrent writes (properly serialized)
- Concurrent updates (no corruption)
- Path traversal prevention

**18 Test Cases**

### 2. File System Validation Tests
**Location**: `tests/HotelDruid.Api.Tests/Services/FileKeyValueStoreFileSystemTests.cs`

Validates actual files on disk:
- Files created with correct structure
- Index files maintained properly
- Atomic writes (temp → rename pattern)
- File deletion and cleanup
- Concurrent file operations
- JSON validity checks

**14 Test Cases**

### 3. API Integration Tests
**Location**: `tests/HotelDruid.Api.Tests/Integration/RoomsApiTests.cs`

Tests HTTP endpoints:
- POST /api/rooms (create)
- GET /api/rooms/{id} (read by ID)
- GET /api/rooms?name=X (read by name)
- GET /api/rooms (list all)
- PUT /api/rooms/{id} (update)
- DELETE /api/rooms/{id} (delete)
- Error cases (409 conflict, 404 not found, 400 bad request)
- Data integrity

**10 Test Cases**

### 4. Event Logging Infrastructure
**Location**: `src/HotelDruid.Api/Services/ISystemEventLogger.cs`

Structured event logging:
- ISystemEventLogger interface
- WindowsEventLogger implementation
- SIEM-compatible JSON format for application events
- Event ID categorization (1000-5099)
- TestEventCapture for test assertions

### 5. Test Helpers & Setup
**Location**: `tests/HotelDruid.Api.Tests/TestSetup.cs`

Utilities for test infrastructure:
- TestFixture (DI setup)
- XunitLogger (xUnit integration)
- TestEventCapture (event tracking)
- TestEnvironment (file system setup)

### 6. Test Runner Script
**Location**: `scripts/run-tests.ps1`

PowerShell script for test execution:
- Automatic environment setup
- Test execution with proper logging
- TRX report generation
- Test summary output
- Artifact directory management

### 7. Test Dashboard Generator
**Location**: `tests/HotelDruid.Api.Tests/Reporting/TestDashboardGenerator.cs`

Report generation:
- HTML dashboard with stats and visual indicators
- JSON report for CI/CD integration
- Performance metrics
- Pass/fail summary
- Error details

### 8. Documentation
**Location**: `docs/TEST_INFRASTRUCTURE.md`

Complete reference guide:
- Test stack overview
- Test suite descriptions
- SIEM integration details
- Running tests (multiple ways)
- Regression testing
- Troubleshooting

---

## Test Execution Flow

```
1. Initialize Test Environment
   ├─ Create temp directory
   ├─ Set up DI container
   └─ Configure logging

2. Run Test Case
   ├─ Perform setup
   ├─ Execute test
   ├─ Assert results
   └─ Log events

3. Cleanup & Report
   ├─ Clean temp directory
   ├─ Dispose resources
   └─ Generate report

4. Summary
   ├─ Count passed/failed
   ├─ Calculate metrics
   └─ Output results
```

---

## File System Layout After Tests

Each test creates a temporary directory structure:

```
C:\Users\{user}\AppData\Local\Temp\HotelDruid-test-{guid}/
├── collections/
│   ├── rooms/
│   │   ├── _index.json                    ← Index file
│   │   ├── a1b2c3d4e5f6g7h8i9j0k1l2.json  ← Data files
│   │   └── b2c3d4e5f6g7h8i9j0k1l2m3.json
│   └── guests/
│       ├── _index.json
│       └── ...
└── ledger/
    └── 2026/12/14/
        ├── _snapshot.json                 ← Daily snapshot
        ├── _seq_001.json                  ← Incremental entries
        └── _seq_002.json
```

**Cleanup**: Temp directories are automatically deleted after each test.

---

## SIEM Integration

Current scope:
- The documented support is structured application logging, not dedicated SIEM HTTP endpoints.
- Reverse proxy and Keycloak logs are separate deployment concerns and can be shipped to the same SIEM externally.
- End-to-end correlation fields such as trace IDs, correlation IDs, and authenticated user context across proxy, identity provider, and API are not described as implemented in the current code path.

### Event Log Format

Events are logged as structured JSON:

```json
{
  "EventType": "DATA_OPERATION",
  "OperationType": "CREATE",
  "Collection": "rooms",
  "DocumentId": "a1b2c3d4e5f6g7h8i9j0k1l2",
  "DocumentName": "Room-1",
  "User": "staff@hotel.com",
  "Timestamp": "2026-02-15T10:30:45.123Z",
  "Details": null
}
```

### SIEM Consumption

These examples assume your deployment forwards Windows Event Log output, reverse proxy logs, and identity-provider logs into the same SIEM. The repository currently documents only the application event format.

**Splunk**:
```spl
source="HotelDruid" | spath path=EventType | stats count by EventType, OperationType
```

**ELK Stack**:
```json
{
  "_source": {
    "EventType": "DATA_OPERATION",
    "Timestamp": "2026-02-15T10:30:45.123Z"
  }
}
```

**Windows Event Log**:
```powershell
Get-EventLog -LogName Application -Source HotelDruid | 
  Where-Object { $_.Message -match 'DATA_OPERATION' }
```

---

## Test Results Files

After running tests, check these locations:

```
artifacts/test-results/
├── test-results.trx           ← Visual Studio Test Results
├── report-metadata.json       ← Test execution metadata
├── dashboard.html             ← HTML dashboard (if generated)
└── results.json               ← JSON report (if generated)
```

### Accessing Results

**In Visual Studio**:
- Test Explorer → Test → Open Recent Test Results

**In Browser**:
```powershell
Start-Process "artifacts/test-results/dashboard.html"
```

**In Command Line**:
```powershell
cat artifacts/test-results/results.json
```

---

## Example Test Output

```
=================================================
HotelDruid API - Test Suite
=================================================
Repository: c:\Users\rolfe\git\own\Villa-Annunziata\HotelDruid
Configuration: Debug

Running tests...

Test Summary:
  Total:    25
  Passed:   25
  Failed:   0
  Skipped:  0

===================================================
Test run completed! (exit code: 0)
===================================================

Results:
  TRX Report:   artifacts/test-results/test-results.trx
  Metadata:     artifacts/test-results/report-metadata.json
```

---

## Common Issues & Solutions

### "File is locked by another process"
- Close any editors opening test files
- Restart Visual Studio
- Manually clean temp directories:
```powershell
Remove-Item C:\Users\$env:USERNAME\AppData\Local\Temp\HotelDruid-test-* -Recurse
```

### "Permission denied" writing to Event Log
- Running as Administrator
- Or disable Event Log for local tests (use TestEventCapture instead)

### Tests timeout
- Increase WebApplicationFactory timeout in test code
- Check for port conflicts
- Disable antivirus scanning temporarily

### Cannot find artifacts directory
- Create it manually: `mkdir artifacts/test-results`
- Or run tests (runner script creates it automatically)

---

## Running Tests in CI/CD

### GitHub Actions
```yaml
- name: Run Tests
  run: dotnet test --logger "trx;LogFileName=test-results.trx"

- name: Upload Results
  uses: actions/upload-artifact@v2
  with:
    name: test-results
    path: "**/test-results.trx"
```

### Azure Pipelines
```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    arguments: '--configuration Release --logger trx'
  displayName: 'Run Tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/test-results.trx'
  displayName: 'Publish Results'
```

---

## Next Steps

The test infrastructure is now ready to support ongoing development:

1. **Continue with Phase 1B**: Implement repositories using same test pattern
2. **Add more endpoints**: Follow the same API test structure
3. **Monitor test coverage**: Track what's being tested
4. **Run regression tests**: Before each release
5. **Monitor SIEM inputs**: Collect application logs and, where deployed, proxy and identity-provider logs

For questions about specific tests, see `docs/TEST_INFRASTRUCTURE.md` for detailed reference.

