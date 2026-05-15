using HotelDruid.Migration;
using System.IO.Compression;
using System.Text.Json;
using Xunit;

namespace HotelDruid.Migration.Tests;

public class ExportPackageBuilderTests
{
    [Fact]
    public async Task BuildAsync_ProducesZipWithManifestAndTableFiles()
    {
        var tables = new List<(string, List<Dictionary<string, object?>>)>
        {
            ("rooms", new List<Dictionary<string, object?>>
            {
                new() { ["id"] = "1", ["name"] = "Room A" },
                new() { ["id"] = "2", ["name"] = "Room B" }
            }),
            ("clients", new List<Dictionary<string, object?>>())
        };

        var tempFile = Path.Combine(Path.GetTempPath(), $"test-export-{Guid.NewGuid():N}.zip");
        try
        {
            var builder = new ExportPackageBuilder();
            await builder.BuildAsync(tables, tempFile);

            Assert.True(File.Exists(tempFile));
            using var archive = ZipFile.OpenRead(tempFile);

            var entries = archive.Entries.Select(e => e.Name).ToList();
            Assert.Contains("manifest.json", entries);
            Assert.Contains("rooms.json", entries);
            Assert.Contains("clients.json", entries);

            // Validate manifest structure
            var manifestEntry = archive.GetEntry("manifest.json")!;
            using var ms = new MemoryStream();
            manifestEntry.Open().CopyTo(ms);
            var manifest = JsonDocument.Parse(ms.ToArray());
            Assert.Equal("1.0", manifest.RootElement.GetProperty("version").GetString());
            Assert.Equal(2, manifest.RootElement.GetProperty("tables").GetArrayLength());

            // Validate rooms.json
            var roomsEntry = archive.GetEntry("rooms.json")!;
            using var ms2 = new MemoryStream();
            roomsEntry.Open().CopyTo(ms2);
            var roomsDoc = JsonDocument.Parse(ms2.ToArray());
            Assert.Equal("rooms", roomsDoc.RootElement.GetProperty("table_name").GetString());
            Assert.Equal(2, roomsDoc.RootElement.GetProperty("rows").GetArrayLength());
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task BuildAsync_EmptyTables_ProducesValidZip()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-empty-{Guid.NewGuid():N}.zip");
        try
        {
            var builder = new ExportPackageBuilder();
            await builder.BuildAsync(new List<(string, List<Dictionary<string, object?>>)>(), tempFile);
            Assert.True(File.Exists(tempFile));
            using var archive = ZipFile.OpenRead(tempFile);
            Assert.Single(archive.Entries); // only manifest.json
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}

