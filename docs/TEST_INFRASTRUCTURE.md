# HotelDruid Test Infrastructure

## Overview

Comprehensive test suite with multiple layers of validation:
- **Unit Tests**: FileKeyValueStore, IdGenerator with file system validation
- **Integration Tests**: API endpoints with WebApplicationFactory
- **Event Logging**: Structured application events suitable for SIEM ingestion
- **Test Dashboard**: HTML and JSON reports for CI/CD

---

## Test Stack

### Frameworks
- **xUnit**: Test framework with parallelization support
- **Moq**: Mocking for dependencies
- **WebApplicationFactory**: ASP.NET Core integration testing
- **IAsyncLifetime**: Proper async setup/teardown

### Logging & Instrumentation
- **ILogger**: Structured logging throughout tests
- **ISystemEventLogger**: SIEM-compatible application event logging
- **XunitLogger**: Integration between xUnit output and ILogger

---

## Test Suites

### 1. Unit Tests: FileKeyValueStore (`FileKeyValueStoreTests.cs`)

**Focus**: Core storage operations with concurrency

**Test Cases**:
- ✅ ID generation (unique, 26-char, URL-safe)
- ✅ CRUD operations (Create, Read, Update, Delete)
- ✅ Index management (mapping, rebuild, validation)
- ✅ Concurrent writes (10+ simultaneous, serialized)
- ✅ Concurrent updates without corruption
- ✅ Path traversal prevention
- ✅ Edge cases (nulls, missing documents, empty collections)

**Run**:
```bash
dotnet test --filter "ClassName=FileKeyValueStoreTests"
```

### 2. File System Tests: FileKeyValueStore (`FileKeyValueStoreFileSystemTests.cs`)

**Focus**: Actual file creation, persistence, validation

**Test Cases**:
- ✅ Creates files on disk with correct structure
- ✅ Creates and maintains index files
- ✅ Modifies files atomically (no corruption)
- ✅ Removes files and updates indexes
- ✅ Index rebuild from disk files
- ✅ Valid JSON structure in files
- ✅ Atomic writes (no temp files left behind)
- ✅ Concurrent writes produce valid files
- ✅ File content matches in-memory state

**Unique Features**:
- Validates actual JSON on disk
- Checks file permissions
- Verifies atomic write pattern (temp → rename)
- Detects orphaned temp files

**Run**:
```bash
dotnet test --filter "ClassName=FileKeyValueStoreFileSystemTests"
```

### 3. API Integration Tests: Rooms (`RoomsApiTests.cs`)

**Focus**: HTTP endpoints for room CRUD

**Test Cases**:
- ✅ POST /api/rooms → 201 Created
- ✅ GET /api/rooms/{id} → 200 Ok
- ✅ GET /api/rooms?name=X → 200 Ok (by name)
- ✅ GET /api/rooms → 200 Ok (list all)
- ✅ PUT /api/rooms/{id} → 200 Ok (update)
- ✅ DELETE /api/rooms/{id} → 204 NoContent
- ✅ Duplicate name → 409 Conflict
- ✅ Non-existent ID → 404 NotFound
- ✅ Invalid input → 400 BadRequest
- ✅ Data integrity (create → read → verify)

**Features**:
- Uses WebApplicationFactory for isolation
- Full environment setup/teardown per test
- Temp data directory per test run
- Cleans up automatically

**Run**:
```bash
dotnet test --filter "ClassName=RoomsApiTests"
```

### 4. Ledger API Tests (`LedgerApiTests.cs`)

**Focus**: Ledger snapshots and entries

**Test Cases**:
- ✅ Get snapshot for date
- ✅ Get entries for date range
- ✅ Handles missing snapshots gracefully

**Run**:
```bash
dotnet test --filter "ClassName=LedgerApiTests"
```

---

## Event Logging & SIEM Integration

### ISystemEventLogger Interface

Structured event logging with categories:

**Event IDs by Category**:
```
1000-1099: System initialization and health
2000-2099: Authentication and authorization
3000-3099: Data operations (CRUD)
4000-4099: Ledger and transactions
5000-5099: Errors and exceptions
```

**Implementation: WindowsEventLogger**

Events are logged as JSON for SIEM consumption:

```json
{
  "EventType": "DATA_OPERATION",
  "OperationType": "CREATE",
  "Collection": "rooms",
  "DocumentId": "a1b2c3d4e5...",
  "DocumentName": "Room-1",
  "User": "staff@hotel.com",
  "Timestamp": "2026-02-15T10:30:45.123Z",
  "Details": "..."
}
```

### SIEM Integration

Current implementation notes:
- The API exposes no dedicated SIEM ingestion or query endpoints.
- `WindowsEventLogger` defines a structured JSON event format, but it is not yet wired into end-to-end request correlation across reverse proxy, Keycloak, and API.
- Reverse proxy and Keycloak logs can still be collected separately by a SIEM platform, but that cross-system correlation must be configured in the deployment stack.

The structured JSON format makes logs consumable by:
- **Splunk**: Parse JSON events, create dashboards
- **ELK Stack**: Elasticsearch index, Kibana visualizations
- **Azure Monitor**: Stream to Log Analytics workspace
- **Datadog**: Custom metrics from event fields
- **Windows Event Log**: Native integration on Windows

**Example: Retrieve all CREATE operations**:
```powershell
Get-EventLog -LogName Application -Source HotelDruid | 
  Where-Object { $_.Message -match '"OperationType":"CREATE"' }
```

This should be read as log-format guidance, not as evidence of full distributed tracing. Correlation IDs, trace IDs, and authenticated user propagation across all layers are not documented as implemented in the current API startup path.

---

## Running Tests

### Run All Tests
```powershell
# From repo root
dotnet test

# With verbose output
dotnet test --verbosity=detailed

# With coverage
dotnet test /p:CollectCoverage=true
```

### Run Specific Test Suite
```powershell
# Unit tests only
dotnet test --filter "ClassName=FileKeyValueStoreTests"

# File system tests only
dotnet test --filter "ClassName=FileKeyValueStoreFileSystemTests"

# API tests only
dotnet test --filter "ClassName=RoomsApiTests"

# Filter by test name
dotnet test --filter "TestName~ConcurrentWrites"
```

### Run with Custom Test Runner Script
```powershell
cd scripts
.\run-tests.ps1                    # Run all tests
.\run-tests.ps1 -Configuration Release
.\run-tests.ps1 -Filter "FileKeyValueStore"
.\run-tests.ps1 -OpenDashboard    # Generate and open dashboard
```

---

## Test Environment Setup

### Per-Test Isolation

Each test:
1. Creates a unique temp directory (`C:\Users\{user}\AppData\Local\Temp\HotelDruid-test-{guid}`)
2. Sets up full environment (FileKeyValueStore, services, etc.)
3. Runs the test
4. Cleans up the temp directory
5. Disposes resources

**IAsyncLifetime Pattern**:
```csharp
public async Task InitializeAsync()
{
    // Setup
    _tempDataRoot = Path.Combine(Path.GetTempPath(), $"HotelDruid-tests-{Guid.NewGuid()}");
    Directory.CreateDirectory(_tempDataRoot);
    _store = new FileKeyValueStore(_tempDataRoot, _mockLogger.Object);
    await Task.CompletedTask;
}

public async Task DisposeAsync()
{
    _store?.Dispose();
    Directory.Delete(_tempDataRoot, recursive: true);
    await Task.CompletedTask;
}
```

### WebApplicationFactory Integration Tests

Full HTTP testing without network calls:

```csharp
_factory = new WebApplicationFactory<Program>()
    .WithWebHostBuilder(builder =>
    {
        builder.UseSetting("DataRoot", _testDataRoot);
    });

_client = _factory.CreateClient();
```

---

## Assertions & File Validation

### File Existence Checks
```csharp
var filePath = Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json");
Assert.True(File.Exists(filePath), $"File not found: {filePath}");
```

### JSON Structure Validation
```csharp
var fileContent = await File.ReadAllTextAsync(filePath);
var parsed = JsonSerializer.Deserialize<TestDocument>(fileContent);
Assert.NotNull(parsed);
Assert.Equal("Room1", parsed!.Name);
```

### File Content Verification
```csharp
var fileContent = await File.ReadAllTextAsync(filePath);
var json = JsonDocument.Parse(fileContent);  // Throws if invalid
Assert.True(json.RootElement.TryGetProperty("name", out var nameElem));
```

---

## Test Dashboards & Reports

### HTML Dashboard
```bash
# Tests generate HTML dashboard (artifacts/test-results/dashboard.html)
Start-Process "artifacts/test-results/dashboard.html"
```

**Features**:
- Summary stats (total, passed, failed, pass rate)
- Per-test results table
- Performance metrics (slowest tests)
- Visual indicators (green/red/yellow)
- Expandable error details

### JSON Report
```bash
# Structured report for CI/CD integration
cat artifacts/test-results/results.json
```

**Fields**:
- Timestamp
- Test summary (counts, pass rate)
- Individual test results
- Duration metrics
- Error messages

### TRX (Visual Studio Test Results)
```bash
# Standard Visual Studio format
artifacts/test-results/test-results.trx
```

Open in Visual Studio Test Explorer for detailed analysis.

---

## Regression Testing

### Track Test Results Over Time

```powershell
# Save baseline
Copy-Item artifacts/test-results/results.json artifacts/baseline-results.json

# Run tests for new version
.\run-tests.ps1

# Compare
Compare-Object -ReferenceObject (Get-Content artifacts/baseline-results.json) `
                -DifferenceObject (Get-Content artifacts/test-results/results.json)
```

### CI/CD Integration

Tests can be run in CI pipelines:

**GitHub Actions**:
```yaml
- name: Run Tests
  run: dotnet test --logger "trx;LogFileName=results.trx"

- name: Publish Test Results
  if: always()
  uses: dorny/test-reporter@v1
  with:
    name: Test Results
    path: '**/test-results/*.trx'
    reporter: 'dotnet trx'
```

**Azure Pipelines**:
```yaml
- task: DotNetCoreCLI@2
  inputs:
    command: 'test'
    arguments: '--logger trx'
  condition: always()

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/test-results.trx'
  condition: always()
```

---

## Test Data Management

### Creating Test Data
```csharp
var doc = new TestDocument { Name = "Room1", Value = 42 };
var id = await _store.CreateAsync("rooms", "Room1", doc);

// Validate file created
Assert.True(File.Exists(Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json")));
```

### Cleanup Strategies

**Per-Test Cleanup** (default):
```csharp
public async Task DisposeAsync()
{
    Directory.Delete(_tempDataRoot, recursive: true);
}
```

**Manual Cleanup** (if needed):
```powershell
# Remove temp test directories older than 1 day
Get-ChildItem C:\Users\{user}\AppData\Local\Temp\HotelDruid-test-* -Directory |
  Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-1) } |
  Remove-Item -Recurse
```

---

## Troubleshooting

### Tests Fail with Permission Denied
**Cause**: Antivirus or file lock
**Solution**: Close any editors opening test files, disable real-time scanning temporarily

### Temp Directory Not Cleaned
**Cause**: Process still holding file handles
**Solution**: Run cleanup script or restart Visual Studio

### API Tests Timeout
**Cause**: Port conflict or slow machine
**Solution**: Increase WebApplicationFactory timeout in test setup

### Event Logging Not Working
**Cause**: Application event log access denied
**Solution**: Ensure running as admin or disable Windows Event Log integration for local tests

---

## Best Practices

✅ **Do**:
- Use `IAsyncLifetime` for proper async setup/teardown
- Create unique temp directories per test
- Validate both in-memory state AND file system
- Test concurrent operations
- Clean up resources in DisposeAsync
- Use structured logging with event types

❌ **Don't**:
- Share test state between tests (use IAsyncLifetime not IDisposable)
- Hardcode paths (use temp directories)
- Rely on test execution order
- Log sensitive data (passwords, tokens)
- Leave temp files behind
- Use real database in unit tests

---

## References

- [xUnit Documentation](https://xunit.net/)
- [WebApplicationFactory](https://docs.microsoft.com/aspnet/core/test/integration-tests)
- [Structured Logging](https://docs.microsoft.com/dotnet/core/extensions/logging)
- [SIEM Log Format Standards](https://www.splunk.com/en_us/software/pricing.html)

