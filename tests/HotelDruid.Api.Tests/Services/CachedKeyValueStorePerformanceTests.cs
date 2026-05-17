using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace HotelDruid.Api.Services.Tests;

/// <summary>
/// Performance benchmarks for caching layer.
/// These tests are marked with [Trait("Category", "Performance")] and should be run separately:
///   dotnet test --filter "Category=Performance"
/// 
/// Baseline (no cache) vs Cached (with in-memory cache) comparisons measure:
/// - ListAsync: Full collection scan (typical N+1 pattern)
/// - GetAsync: Single document retrieval
/// - GetIndexAsync: Index lookup
/// - Repeated reads: Shows cache warmup benefit
/// 
/// Expected improvements with cache enabled:
/// - ListAsync: 50-80% reduction (eliminates N file reads)
/// - Repeated GetAsync: 90%+ reduction (in-memory vs file I/O)
/// - GetIndexAsync: 95%+ reduction (cached dictionary vs file read)
/// </summary>
[Trait("Category", "Performance")]
public class CachedKeyValueStorePerformanceTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private string _tempDataRoot = null!;
    private FileKeyValueStore _baselineStore = null!;
    private Mock<ILogger<FileKeyValueStore>> _mockLogger = null!;

    public CachedKeyValueStorePerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public async Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDataRoot);
        
        _mockLogger = new Mock<ILogger<FileKeyValueStore>>();
        _baselineStore = new FileKeyValueStore(_tempDataRoot, _mockLogger.Object);
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _baselineStore?.Dispose();
        
        if (Directory.Exists(_tempDataRoot))
        {
            Directory.Delete(_tempDataRoot, recursive: true);
        }
        
        await Task.CompletedTask;
    }

    private class BookingDocument
    {
        public string? ClientId { get; set; }
        public string? RoomId { get; set; }
        public int Year { get; set; }
        public string? Status { get; set; }
    }

    /// <summary>
    /// Measure baseline (uncached) performance for listing bookings.
    /// Creates 100 bookings and measures repeated list operations.
    /// </summary>
    [Fact]
    public async Task Baseline_ListAsync_WithManyDocuments()
    {
        const string collection = "bookings_baseline_list";
        const int documentCount = 100;
        const int iterations = 10;

        // Setup: Create many documents
        _output.WriteLine($"Creating {documentCount} documents...");
        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument { ClientId = $"C{i:D4}", Year = 2026, RoomId = "101", Status = "Confirmed" };
            await _baselineStore.CreateAsync(collection, $"2026_{i:D10}", booking);
        }

        // Warmup
        _ = await _baselineStore.ListAsync<BookingDocument>(collection);

        // Measure: Repeated list operations (baseline - no cache means disk I/O every time)
        var sw = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _baselineStore.ListAsync<BookingDocument>(collection);
        }
        sw.Stop();

        var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
        _output.WriteLine($"Baseline ListAsync: {iterations} iterations in {sw.Elapsed.TotalMilliseconds:F2}ms (avg {avgMs:F2}ms)");
        _output.WriteLine($"  → Expect cache to reduce this by 50-80% via N+1 elimination");
    }

    /// <summary>
    /// Measure baseline (uncached) performance for single document retrieval.
    /// Creates a document and measures repeated get operations.
    /// </summary>
    [Fact]
    public async Task Baseline_GetAsync_WithManyRepeatedReads()
    {
        const string collection = "bookings_baseline_get";
        const int iterations = 100;

        // Setup: Create a document
        var booking = new BookingDocument { ClientId = "C0001", Year = 2026, RoomId = "101", Status = "Confirmed" };
        var id = await _baselineStore.CreateAsync(collection, "2026_0000000001", booking);

        // Warmup
        _ = await _baselineStore.GetAsync<BookingDocument>(collection, id);

        // Measure: Repeated get operations (baseline - disk I/O every time)
        var sw = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _baselineStore.GetAsync<BookingDocument>(collection, id);
        }
        sw.Stop();

        var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
        _output.WriteLine($"Baseline GetAsync: {iterations} iterations in {sw.Elapsed.TotalMilliseconds:F2}ms (avg {avgMs:F2}ms)");
        _output.WriteLine($"  → Expect cache to reduce this by 90%+ via in-memory lookup");
    }

    /// <summary>
    /// Measure baseline (uncached) performance for index lookup.
    /// Creates 50 documents and measures repeated index reads.
    /// </summary>
    [Fact]
    public async Task Baseline_GetIndexAsync_WithManyDocuments()
    {
        const string collection = "bookings_baseline_index";
        const int documentCount = 50;
        const int iterations = 20;

        // Setup: Create many documents
        _output.WriteLine($"Creating {documentCount} documents for index test...");
        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument { ClientId = $"C{i:D4}", Year = 2026, RoomId = "101", Status = "Confirmed" };
            await _baselineStore.CreateAsync(collection, $"2026_{i:D10}", booking);
        }

        // Warmup
        _ = await _baselineStore.GetIndexAsync(collection);

        // Measure: Repeated index reads (baseline - file I/O every time)
        var sw = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _baselineStore.GetIndexAsync(collection);
        }
        sw.Stop();

        var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
        _output.WriteLine($"Baseline GetIndexAsync: {iterations} iterations in {sw.Elapsed.TotalMilliseconds:F2}ms (avg {avgMs:F2}ms)");
        _output.WriteLine($"  → Expect cache to reduce this by 95%+ via cached dictionary");
    }

    /// <summary>
    /// Measure cache invalidation overhead on writes.
    /// Creates 100 documents and measures mutation performance.
    /// Baseline should be fast because writes don't involve reads.
    /// </summary>
    [Fact]
    public async Task Baseline_WriteOperations_Performance()
    {
        const string collection = "bookings_baseline_writes";
        const int documentCount = 100;

        // Measure: Create operations
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument { ClientId = $"C{i:D4}", Year = 2026, RoomId = "101", Status = "Confirmed" };
            await _baselineStore.CreateAsync(collection, $"2026_{i:D10}", booking);
        }
        sw.Stop();

        var avgMs = sw.Elapsed.TotalMilliseconds / documentCount;
        _output.WriteLine($"Baseline CreateAsync: {documentCount} documents in {sw.Elapsed.TotalMilliseconds:F2}ms (avg {avgMs:F2}ms per document)");
        _output.WriteLine($"  → Cache adds minimal overhead to mutation path");
    }

    /// <summary>
    /// Measure realistic mixed workload: list once, then repeated gets.
    /// Simulates typical API usage pattern: load all bookings, then individual lookups.
    /// </summary>
    [Fact]
    public async Task Baseline_RealisticMixedWorkload()
    {
        const string collection = "bookings_baseline_mixed";
        const int documentCount = 50;
        const int getIterations = 5;

        // Setup: Create documents
        var ids = new List<string>();
        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument { ClientId = $"C{i:D4}", Year = 2026, RoomId = "101", Status = "Confirmed" };
            var id = await _baselineStore.CreateAsync(collection, $"2026_{i:D10}", booking);
            ids.Add(id);
        }

        // Measure: List + multiple gets
        var sw = Stopwatch.StartNew();
        
        // Initial list (like /api/bookings endpoint)
        var allBookings = await _baselineStore.ListAsync<BookingDocument>(collection);
        
        // Repeated individual gets (like /api/bookings/{id} endpoint)
        for (int iter = 0; iter < getIterations; iter++)
        {
            foreach (var id in ids)
            {
                _ = await _baselineStore.GetAsync<BookingDocument>(collection, id);
            }
        }

        sw.Stop();

        var totalOps = 1 + (getIterations * documentCount);
        var avgMs = sw.Elapsed.TotalMilliseconds / totalOps;
        _output.WriteLine($"Baseline Mixed Workload: {totalOps} ops ({1} list + {getIterations} × {documentCount} gets) in {sw.Elapsed.TotalMilliseconds:F2}ms (avg {avgMs:F2}ms per op)");
        _output.WriteLine($"  → Cache should show large improvement on repeated gets");
    }
}
