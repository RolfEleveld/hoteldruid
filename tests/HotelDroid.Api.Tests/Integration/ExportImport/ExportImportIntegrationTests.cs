using HotelDroid.Api.Models;
using HotelDroid.Api.Services.ExportImport;
using HotelDroid.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HotelDroid.Api.Tests.Integration.ExportImport;

/// <summary>
/// Integration tests for complete export/import cycle
/// Tests full flow: rooms → canonical → package → ZIP → extract → import → validate
/// </summary>
public class ExportImportIntegrationTests : IAsyncLifetime
{
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly IPackageBuilder _packageBuilder;
    private readonly IZipBuilder _zipBuilder;
    private readonly IApiDistributor _apiDistributor;
    private readonly IExportService _exportService;
    private readonly IImportService _importService;
    private readonly MockFileKeyValueStore _roomsStore;
    private readonly ILogger<ExportImportIntegrationTests> _logger;
    private readonly string _tempDirectory;

    public ExportImportIntegrationTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"export_import_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);

        _logger = LoggingHelper.CreateLogger<ExportImportIntegrationTests>();

        // Create configuration with temp directories
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Storage:ExportDirectory"] = Path.Combine(_tempDirectory, "exports"),
                ["Storage:ImportDirectory"] = Path.Combine(_tempDirectory, "imports")
            })
            .Build();

        // Initialize services
        _roomsStore = new MockFileKeyValueStore();
        _canonicalMapper = new CanonicalMapper(LoggingHelper.CreateLogger<CanonicalMapper>());
        _packageBuilder = new PackageBuilder(_canonicalMapper, LoggingHelper.CreateLogger<PackageBuilder>());
        _zipBuilder = new ZipBuilder(LoggingHelper.CreateLogger<ZipBuilder>(), config);
        _apiDistributor = new ApiDistributor(_roomsStore, _canonicalMapper, LoggingHelper.CreateLogger<ApiDistributor>());
        _exportService = new ExportService(_canonicalMapper, _packageBuilder, _zipBuilder, _roomsStore, LoggingHelper.CreateLogger<ExportService>());
        _importService = new ImportService(_zipBuilder, _canonicalMapper, _apiDistributor, LoggingHelper.CreateLogger<ImportService>());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        try { Directory.Delete(_tempDirectory, true); } catch { }
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Export_CreatesValidZipPackage()
    {
        // Arrange
        var rooms = new[]
        {
            new RoomDto("room1", "Room 1", 2, "1", "101", null, null, "S", null, null),
            new RoomDto("room2", "Room 2", 4, "1", "102", 1, null, "S", "room1", "Near stairs")
        };

        // Act
        var canonical = _canonicalMapper.ToCanonical(rooms);
        var package = await _packageBuilder.BuildAsync(
            new() { ["rooms"] = canonical },
            new ExportRequest());
        var zipPath = await _zipBuilder.CreateZipAsync(package);

        // Assert
        Assert.True(File.Exists(zipPath));
        Assert.True(new FileInfo(zipPath).Length > 0);
        
        // Verify ZIP contents
        using (var archive = System.IO.Compression.ZipFile.OpenRead(zipPath))
        {
            var entries = archive.Entries.Select(e => e.Name).ToList();
            Assert.Contains("manifest.json", entries);
            Assert.Contains("rooms.json", entries);
        }
    }

    [Fact]
    public async Task ExportThenImport_PreservesAllRoomData()
    {
        // Arrange
        var originalRooms = new[]
        {
            new RoomDto(
                "room1", "Suite 1", 4, "2", "201", 1, 2, "S", "room2,room3",
                "Deluxe with view"),
            new RoomDto(
                "room2", "Double 1", 2, "2", "202", 2, null, "S", "room1,room3", null)
        };

        // Create export
        var canonical = _canonicalMapper.ToCanonical(originalRooms);
        var package = await _packageBuilder.BuildAsync(
            new() { ["rooms"] = canonical },
            new ExportRequest());
        var zipPath = await _zipBuilder.CreateZipAsync(package);

        // Import the ZIP
        // For this test, we'll manually extract and reimport to simulate the full flow
        var tempExtractDir = Path.Combine(_tempDirectory, "extracted");
        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempExtractDir);

        // Read the canonical data
        var roomsJson = await File.ReadAllTextAsync(
            Path.Combine(tempExtractDir, "data", "tables", "rooms.json"));
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        var importedCanonical = System.Text.Json.JsonSerializer.Deserialize<CanonicalData>(roomsJson, jsonOptions);
        // Ensure TableName is set for FromCanonical
        if (importedCanonical is not null && string.IsNullOrEmpty(importedCanonical.TableName))
        {
            importedCanonical = importedCanonical with { TableName = "rooms" };
        }
        var reimportedRooms = _canonicalMapper.FromCanonical(importedCanonical);

        // Assert all data preserved
        Assert.Equal(2, reimportedRooms.Length);
        Assert.Equal("room1", reimportedRooms[0].Id);
        Assert.Equal("Suite 1", reimportedRooms[0].Name);
        Assert.Equal(4, reimportedRooms[0].Capacity);
        Assert.Equal("2", reimportedRooms[0].FloorNumber);
        Assert.Equal("201", reimportedRooms[0].HouseNumber);
        Assert.Equal(1, reimportedRooms[0].Priority);
        Assert.Equal(2, reimportedRooms[0].SecondaryPriority);
        Assert.Equal("S", reimportedRooms[0].HasBeds);
        Assert.Equal("room2,room3", reimportedRooms[0].NeighboringRooms);
        Assert.Equal("Deluxe with view", reimportedRooms[0].Comments);
    }

    [Fact]
    public async Task ExportFormatCompatibility_WithHotelDroidNames()
    {
        // Arrange - simulate export from HotelDroid with Italian names
        var hoteldruidCanonical = new CanonicalData(
            TableName: "appartamenti",  // Italian name
            RowCount: 2,
            Rows: new()
            {
                new()
                {
                    ["idappartamenti"] = "app1",
                    ["maxoccupanti"] = 2,
                    ["numpiano"] = "1",
                    ["numcasa"] = "101"
                },
                new()
                {
                    ["idappartamenti"] = "app2",
                    ["maxoccupanti"] = 4,
                    ["numpiano"] = "1",
                    ["numcasa"] = "102",
                    ["priorita"] = 1
                }
            },
            Columns: new()
        );

        // Act - import HotelDroid format
        var rooms = _canonicalMapper.FromCanonical(hoteldruidCanonical);

        // Assert - should work despite Italian naming
        Assert.Equal(2, rooms.Length);
        Assert.Equal("app1", rooms[0].Id);
        Assert.Equal(2, rooms[0].Capacity);
        Assert.Equal("1", rooms[0].FloorNumber);
        Assert.Equal("101", rooms[0].HouseNumber);
        Assert.Equal(4, rooms[1].Capacity);
        Assert.Equal(1, rooms[1].Priority);
    }

    [Fact]
    public async Task ImportRoomsThroughApiDistributor()
    {
        // Arrange
        var canonicalData = new Dictionary<string, CanonicalData>
        {
            ["rooms"] = new CanonicalData(
                TableName: "rooms",
                RowCount: 2,
                Rows: new()
                {
                    new() { ["id"] = "r1", ["name"] = "Room 1", ["capacity"] = 2 },
                    new() { ["id"] = "r2", ["name"] = "Room 2", ["capacity"] = 4 }
                },
                Columns: new()
            )
        };

        // Act
        var result = await _apiDistributor.DistributeAsync(canonicalData);

        // Assert
        Assert.True(result.ByTable["rooms"].Success);
        Assert.Equal(2, result.ByTable["rooms"].ImportedCount);
        Assert.Equal(2, result.TotalImported);

        // Verify rooms were stored by name (the key value store uses names as index keys)
        Assert.NotNull(await _roomsStore.GetByNameAsync<RoomDto>("rooms", "Room 1"));
        Assert.NotNull(await _roomsStore.GetByNameAsync<RoomDto>("rooms", "Room 2"));
    }

    [Fact]
    public async Task ValidateImportPackage_DetectsTableStructure()
    {
        // Preamble: Clear any shared state from previous tests
        _roomsStore.DeleteAsync("rooms", "room1").Wait();
        
        // Arrange - create a test ZIP with manifest
        var rooms = new[]
        {
            new RoomDto("room1", "Room 1", 2, "1", "101", null, null, null, null, null)
        };

        var canonical = _canonicalMapper.ToCanonical(rooms);
        var package = await _packageBuilder.BuildAsync(
            new() { ["rooms"] = canonical },
            new ExportRequest());
        var zipPath = await _zipBuilder.CreateZipAsync(package);

        // Create form file from ZIP - use bytes instead of stream to avoid disposal issues
        var zipBytes = await File.ReadAllBytesAsync(zipPath);
        var memoryStream = new MemoryStream(zipBytes);
        var formFile = new FormFileWrapper(memoryStream, "test.zip");

        // Act
        var validation = await _importService.ValidatePackageAsync(formFile);

        // Assert
        Assert.True(validation.Valid, $"Package validation failed with errors: {string.Join(", ", validation.Errors)}");
        Assert.NotEmpty(validation.Tables);
        if (validation.Tables.Count > 0)
        {
            Assert.Equal("rooms", validation.Tables[0].Name);  // Normalized to standard name
            Assert.Equal(1, validation.Tables[0].RowCount);
        }
        
        // Cleanup: Clear test data and temp files after test completes
        try { await _roomsStore.DeleteAsync("rooms", "room1"); } catch { }
    }

    [Fact]
    public async Task MultipleExports_AreIndependent()
    {
        // Arrange
        var rooms1 = new[]
        {
            new RoomDto("room1", "Room 1", 2, "1", "101", null, null, null, null, null)
        };

        var rooms2 = new[]
        {
            new RoomDto("room2", "Room 2", 4, "2", "201", null, null, null, null, null),
            new RoomDto("room3", "Room 3", 3, "2", "202", null, null, null, null, null)
        };

        // Act
        var canonical1 = _canonicalMapper.ToCanonical(rooms1);
        var package1 = await _packageBuilder.BuildAsync(
            new() { ["rooms"] = canonical1 }, new ExportRequest());
        var zip1 = await _zipBuilder.CreateZipAsync(package1);

        var canonical2 = _canonicalMapper.ToCanonical(rooms2);
        var package2 = await _packageBuilder.BuildAsync(
            new() { ["rooms"] = canonical2 }, new ExportRequest());
        var zip2 = await _zipBuilder.CreateZipAsync(package2);

        // Assert
        Assert.NotEqual(zip1, zip2);
        Assert.True(File.Exists(zip1));
        Assert.True(File.Exists(zip2));
        Assert.NotEqual(
            new FileInfo(zip1).Length,
            new FileInfo(zip2).Length);
    }
}

/// <summary>
/// Wrapper to convert FileStream to IFormFile for testing
/// </summary>
public class FormFileWrapper : IFormFile
{
    private readonly byte[] _data;
    private readonly string _fileName;

    public FormFileWrapper(Stream stream, string fileName)
    {
        using (var ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            _data = ms.ToArray();
        }
        _fileName = fileName;
    }

    // Return a fresh stream each time to avoid disposition issues
    public Stream OpenReadStream() => new MemoryStream(_data);
    public void CopyTo(Stream target) => target.Write(_data, 0, _data.Length);
    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
        await target.WriteAsync(_data, 0, _data.Length, cancellationToken);

    public string ContentType => "application/zip";
    public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{_fileName}\"";
    public IHeaderDictionary Headers => new HeaderDictionary();
    public long Length => _data.Length;
    public string Name => "file";
    public string FileName => _fileName;
}
