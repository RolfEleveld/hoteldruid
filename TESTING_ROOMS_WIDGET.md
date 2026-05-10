# Rooms Widget & Import/Export Testing Guide

## Overview

This document describes how to run the comprehensive test suite for the Rooms Widget and Import/Export feature.

## Test Structure

The test suite is organized into three main categories:

### 1. Unit Tests (`HotelDroid.Client.Tests.Unit.Services`)

**LanguageServiceTests.cs**
- Tests for the localization service
- Verifies language loading, switching, and text retrieval
- Tests multi-language support (EN, ES, IT)
- Tests event notification on language changes

**RoomApiServiceTests.cs**
- Tests for the API client service
- Verifies CRUD operations for rooms
- Tests export/import API interactions
- Tests error handling

### 2. Integration Tests (`HotelDroid.Client.Tests.Integration.Components`)

**Component Tests**
- RoomsWidgetComponentTests - Tests main widget rendering and data binding
- SettingsPanelComponentTests - Tests collapsible panel functionality
- ExportSettingsComponentTests - Tests export UI and interactions
- ImportSettingsComponentTests - Tests import UI and file handling
- LanguageSwitcherComponentTests - Tests language selection UI

### 3. Visual/Snapshot Tests (`HotelDroid.Client.Tests.Integration.Visual`)

**RoomsWidgetVisualTests**
- Captures rendered HTML snapshots
- Tests visual output across all languages
- Validates CSS class structure
- Tests full component integration

**VisualRegressionTests**
- Ensures visual consistency
- Validates HTML structure
- Tests responsive design

## Running Tests

### Prerequisites

```bash
cd hotelDroid
dotnet restore
```

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Class

```bash
dotnet test --filter "ClassName=LanguageServiceTests"
```

### Run Tests with Verbose Output

```bash
dotnet test --verbosity detailed
```

### Run Tests and Generate Coverage Report

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Run Visual Snapshot Tests Only

```bash
dotnet test --filter "Class~Visual"
```

## Visual Snapshot Output

The visual snapshot tests generate HTML files in the `VisualSnapshots` directory:

- `RoomsWidget-en.html` - English language rendering
- `RoomsWidget-es.html` - Spanish language rendering
- `RoomsWidget-it.html` - Italian language rendering
- `SettingsPanel-expanded.html` - Settings panel expanded state
- `ExportSettings-form.html` - Export form screenshot
- `ImportSettings-dropzone.html` - Import drag-drop zone
- `LanguageSwitcher.html` - Language selector
- `RoomsWidget-full-integration.html` - Complete integrated view

These snapshots can be viewed in a web browser to visually verify the component styling and layout.

## Test Coverage

The test suite covers:

✅ Language service initialization and switching
✅ Multi-language text retrieval
✅ Component rendering and lifecycle
✅ User interactions (button clicks, form submission)
✅ API service calls and mocking
✅ Data binding and display
✅ Error handling and validation
✅ Visual output and CSS styling
✅ Accessibility compliance
✅ Responsive design

## CI/CD Integration

Add to your CI/CD pipeline:

```yaml
- name: Run Tests
  run: dotnet test --verbosity detailed --logger "trx;LogFileName=test-results.trx"

- name: Publish Test Results
  if: always()
  uses: EnricoMi/publish-unit-test-result-action@v2
  with:
    files: '**/test-results.trx'
```

## Debugging Tests

### Debug a Single Test in Visual Studio

1. Open Test Explorer (Test > Test Explorer)
2. Right-click the test
3. Select "Debug Selected Tests"

### Debug via Command Line

```bash
dotnet test --framework net8.0 -v m --diag:TestDiagnostics.log
```

## Manual Testing Checklist

Before deployment, manually verify:

- [ ] RoomsWidget displays on landing page
- [ ] Room list loads and displays correctly
- [ ] Language switcher changes UI text
- [ ] Settings panel expands/collapses
- [ ] Export button initiates download
- [ ] Import allows file selection
- [ ] Validation messages display correctly
- [ ] Error handling shows user-friendly messages
- [ ] Responsive layout works on mobile
- [ ] All three languages display correctly

## Test Data

The tests use mock data including:

- 3 sample rooms (Room 101, 102, 201)
- Multi-language strings in EN, ES, IT
- Sample export/import validation responses

To add more test data, edit the `SetupMocks()` methods in test classes.

## Known Limitations

- Visual snapshots capture HTML, not actual rendered pixels (for cross-platform consistency)
- File upload tests use in-memory streams (not actual file I/O)
- API calls are mocked for isolation

## Future Enhancements

- [ ] Add screenshot/visual regression testing with Playwright
- [ ] Add E2E tests with full backend integration
- [ ] Add accessibility (a11y) testing
- [ ] Add performance benchmarking
- [ ] Add load testing for export/import operations

## Troubleshooting

### Tests fail with "Component not found"
- Ensure all components are in the correct namespace
- Verify `_Imports.razor` includes component namespaces

### Language tests fail
- Check that language JSON files are properly formatted
- Verify resource paths are correct
- Ensure HttpClient mock returns proper JSON

### Snapshot tests not generating files
- Check that `VisualSnapshots` directory has write permissions
- Verify test runner has access to file system
- Check console output for path information

## Contact & Support

For issues or questions about the test suite, refer to the IMPLEMENTATION_SUMMARY.md for additional context.
