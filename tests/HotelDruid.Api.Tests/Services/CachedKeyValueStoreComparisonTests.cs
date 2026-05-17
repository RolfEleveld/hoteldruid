using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace HotelDruid.Api.Services.Tests;

/// <summary>
/// Comparison tests: CachedKeyValueStoreDecorator vs baseline FileKeyValueStore.
/// Run with: dotnet test --filter "Category=Performance"
/// 
/// These tests measure the same operations with and without cache to quantify improvement.
/// </summary>
[Trait("Category", "Performance")]
public class CachedKeyValueStoreComparisonTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private string _tempDataRoot1 = null!;
    private string _tempDataRoot2 = null!;
    private FileKeyValueStore _baselineStore = null!;
    private CachedKeyValueStoreDecorator _cachedStore = null!;
    private Mock<ILogger<FileKeyValueStore>> _mockLogger1 = null!;
    private Mock<ILogger<CachedKeyValueStoreDecorator>> _mockLogger2 = null!;

    public CachedKeyValueStoreComparisonTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public async Task InitializeAsync()
    {
        _tempDataRoot1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _tempDataRoot2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDataRoot1);
        Directory.CreateDirectory(_tempDataRoot2);
        
        _mockLogger1 = new Mock<ILogger<FileKeyValueStore>>();
        _mockLogger2 = new Mock<ILogger<CachedKeyValueStoreDecorator>>();

        _baselineStore = new FileKeyValueStore(_tempDataRoot1, _mockLogger1.Object);
        var uncachedStore = new FileKeyValueStore(_tempDataRoot2, _mockLogger1.Object);
        _cachedStore = new CachedKeyValueStoreDecorator(uncachedStore, _mockLogger2.Object);
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _baselineStore?.Dispose();

        if (Directory.Exists(_tempDataRoot1)) Directory.Delete(_tempDataRoot1, recursive: true);
        if (Directory.Exists(_tempDataRoot2)) Directory.Delete(_tempDataRoot2, recursive: true);
        
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
    /// Compare ListAsync performance: baseline vs cached.
    /// </summary>
    [Fact]
    public async Task Comparison_ListAsync_Performance()
    {
        const string collection = "bookings_comp_list";
        const int documentCount = 100;
        const int iterations = 10;

        // Setup baseline store
        _output.WriteLine("=== ListAsync Performance Comparison ===");
        _output.WriteLine($"Setup: {documentCount} documents, {iterations} iterations\n");

        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument { ClientId = $"C{i:D4}", Year = 2026, RoomId = "101", Status = "Confirmed" };
            await _baselineStore.CreateAsync(collection, $"2026_{i:D10}", booking);
            await _cachedStore.CreateAsync(collection, $"2026_{i:D10}", booking);
        }

        // Warmup both
        _ = await _baselineStore.ListAsync<BookingDocument>(collection);
        _ = await _cachedStore.ListAsync<BookingDocument>(collection);

        // Measure baseline (no cache)
        var swBaseline = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _baselineStore.ListAsync<BookingDocument>(collection);
        }
        swBaseline.Stop();
        var baselineAvg = swBaseline.Elapsed.TotalMilliseconds / iterations;

        // Measure cached
        var swCached = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _cachedStore.ListAsync<BookingDocument>(collection);
        }
        swCached.Stop();
        var cachedAvg = swCached.Elapsed.TotalMilliseconds / iterations;

        var improvement = ((baselineAvg - cachedAvg) / baselineAvg) * 100;

        _output.WriteLine($"Baseline: {swBaseline.Elapsed.TotalMilliseconds:F2}ms total ({baselineAvg:F2}ms avg)");
        _output.WriteLine($"Cached:   {swCached.Elapsed.TotalMilliseconds:F2}ms total ({cachedAvg:F2}ms avg)");
        _output.WriteLine($"Improvement: {improvement:F1}%\n");

        Assert.True(cachedAvg < baselineAvg, $"Cache should be faster (cached={cachedAvg:F2}ms vs baseline={baselineAvg:F2}ms)");
    }

    /// <summary>
    /// Compare GetAsync performance: baseline vs cached.
    /// </summary>
    [Fact]
    public async Task Comparison_GetAsync_Performance()
    {
        const string collection = "bookings_comp_get";
        const int iterations = 100;

        _output.WriteLine("=== GetAsync Performance Comparison ===");
        _output.WriteLine($"Setup: 1 document, {iterations} iterations\n");

        var booking = new BookingDocument { ClientId = "C0001", Year = 2026, RoomId = "101", Status = "Confirmed" };
        var idBaseline = await _baselineStore.CreateAsync(collection, "2026_0000000001", booking);
        var idCached = await _cachedStore.CreateAsync(collection, "2026_0000000001", booking);

        // Warmup
        _ = await _baselineStore.GetAsync<BookingDocument>(collection, idBaseline);
        _ = await _cachedStore.GetAsync<BookingDocument>(collection, idCached);

        // Measure baseline
        var swBaseline = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _baselineStore.GetAsync<BookingDocument>(collection, idBaseline);
        }
        swBaseline.Stop();
        var baselineAvg = swBaseline.Elapsed.TotalMilliseconds / iterations;

        // Measure cached
        var swCached = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _cachedStore.GetAsync<BookingDocument>(collection, idCached);
        }
        swCached.Stop();
        var cachedAvg = swCached.Elapsed.TotalMilliseconds / iterations;

        var improvement = ((baselineAvg - cachedAvg) / baselineAvg) * 100;

        _output.WriteLine($"Baseline: {swBaseline.Elapsed.TotalMilliseconds:F2}ms total ({baselineAvg:F2}ms avg)");
        _output.WriteLine($"Cached:   {swCached.Elapsed.TotalMilliseconds:F2}ms total ({cachedAvg:F2}ms avg)");
        _output.WriteLine($"Improvement: {improvement:F1}%\n");

        Assert.True(cachedAvg < baselineAvg, $"Cache should be faster (cached={cachedAvg:F2}ms vs baseline={baselineAvg:F2}ms)");
    }

    /// <summary>
    /// Compare GetIndexAsync performance: baseline vs cached.
    /// </summary>
    [Fact]
    public async Task Comparison_GetIndexAsync_Performance()
    {
        const string collection = "bookings_comp_index";
        const int documentCount = 50;
        const int iterations = 20;

        _output.WriteLine("=== GetIndexAsync Performance Comparison ===");
        _output.WriteLine($"Setup: {documentCount} documents, {iterations} iterations\n");

        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument { ClientId = $"C{i:D4}", Year = 2026, RoomId = "101", Status = "Confirmed" };
            await _baselineStore.CreateAsync(collection, $"2026_{i:D10}", booking);
            await _cachedStore.CreateAsync(collection, $"2026_{i:D10}", booking);
        }

        // Warmup
        _ = await _baselineStore.GetIndexAsync(collection);
        _ = await _cachedStore.GetIndexAsync(collection);

        // Measure baseline
        var swBaseline = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _baselineStore.GetIndexAsync(collection);
        }
        swBaseline.Stop();
        var baselineAvg = swBaseline.Elapsed.TotalMilliseconds / iterations;

        // Measure cached
        var swCached = Stopwatch.StartNew();
        for (int iter = 0; iter < iterations; iter++)
        {
            _ = await _cachedStore.GetIndexAsync(collection);
        }
        swCached.Stop();
        var cachedAvg = swCached.Elapsed.TotalMilliseconds / iterations;

        var improvement = ((baselineAvg - cachedAvg) / baselineAvg) * 100;

        _output.WriteLine($"Baseline: {swBaseline.Elapsed.TotalMilliseconds:F2}ms total ({baselineAvg:F2}ms avg)");
        _output.WriteLine($"Cached:   {swCached.Elapsed.TotalMilliseconds:F2}ms total ({cachedAvg:F2}ms avg)");
        _output.WriteLine($"Improvement: {improvement:F1}%\n");

        Assert.True(cachedAvg < baselineAvg, $"Cache should be faster (cached={cachedAvg:F2}ms vs baseline={baselineAvg:F2}ms)");
    }

    /// <summary>
    /// Compare realistic mixed workload.
    /// </summary>
    [Fact]
    public async Task Comparison_RealisticMixedWorkload()
    {
        const string collection = "bookings_comp_mixed";
        const int documentCount = 50;
        const int getIterations = 5;

        _output.WriteLine("=== Mixed Workload Performance Comparison ===");
        _output.WriteLine($"Setup: {documentCount} documents, 1 list + {getIterations}×{documentCount} gets\n");

        var idsBaseline = new List<string>();
        var idsCached = new List<string>();

        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument { ClientId = $"C{i:D4}", Year = 2026, RoomId = "101", Status = "Confirmed" };
            var id1 = await _baselineStore.CreateAsync(collection, $"2026_{i:D10}", booking);
            var id2 = await _cachedStore.CreateAsync(collection, $"2026_{i:D10}", booking);
            idsBaseline.Add(id1);
            idsCached.Add(id2);
        }

        // Measure baseline
        var swBaseline = Stopwatch.StartNew();
        _ = await _baselineStore.ListAsync<BookingDocument>(collection);
        for (int iter = 0; iter < getIterations; iter++)
        {
            foreach (var id in idsBaseline)
            {
                _ = await _baselineStore.GetAsync<BookingDocument>(collection, id);
            }
        }
        swBaseline.Stop();

        // Measure cached
        var swCached = Stopwatch.StartNew();
        _ = await _cachedStore.ListAsync<BookingDocument>(collection);
        for (int iter = 0; iter < getIterations; iter++)
        {
            foreach (var id in idsCached)
            {
                _ = await _cachedStore.GetAsync<BookingDocument>(collection, id);
            }
        }
        swCached.Stop();

        var totalOps = 1 + (getIterations * documentCount);
        var baselineAvg = swBaseline.Elapsed.TotalMilliseconds / totalOps;
        var cachedAvg = swCached.Elapsed.TotalMilliseconds / totalOps;
        var improvement = ((swBaseline.Elapsed.TotalMilliseconds - swCached.Elapsed.TotalMilliseconds) / swBaseline.Elapsed.TotalMilliseconds) * 100;

        _output.WriteLine($"Baseline: {swBaseline.Elapsed.TotalMilliseconds:F2}ms total ({baselineAvg:F3}ms avg per op)");
        _output.WriteLine($"Cached:   {swCached.Elapsed.TotalMilliseconds:F2}ms total ({cachedAvg:F3}ms avg per op)");
        _output.WriteLine($"Overall Improvement: {improvement:F1}%\n");

        Assert.True(swCached.Elapsed < swBaseline.Elapsed, $"Cache should be faster for mixed workload");
    }

    [Fact]
    public async Task Comparison_CleanStart_Vs_WarmStart_Performance()
    {
        const string collection = "bookings_comp_startup";
        const int documentCount = 120;
        const int restartIterations = 5;

        _output.WriteLine("=== Clean Start vs Warm Start Comparison ===");
        _output.WriteLine($"Setup: {documentCount} documents, {restartIterations} clean restarts\n");

        for (int i = 0; i < documentCount; i++)
        {
            var booking = new BookingDocument
            {
                ClientId = $"C{i:D4}",
                Year = 2026,
                RoomId = "101",
                Status = "Confirmed"
            };

            await _cachedStore.CreateAsync(collection, $"2026_{i:D10}", booking);
        }

        // Clean start: simulate process restart by constructing a new cache decorator each run.
        var cleanStartDurations = new List<double>(restartIterations);
        for (int i = 0; i < restartIterations; i++)
        {
            var uncachedStore = new FileKeyValueStore(_tempDataRoot2, _mockLogger1.Object);
            var coldCache = new CachedKeyValueStoreDecorator(
                uncachedStore,
                _mockLogger2.Object,
                invalidationRootPath: _tempDataRoot2,
                versionCheckIntervalMilliseconds: 0);

            var swCold = Stopwatch.StartNew();
            var docs = await coldCache.ListAsync<BookingDocument>(collection);
            swCold.Stop();

            Assert.Equal(documentCount, docs.Count);
            cleanStartDurations.Add(swCold.Elapsed.TotalMilliseconds);
            uncachedStore.Dispose();
        }

        var cleanStartAvg = cleanStartDurations.Average();

        // Warm start: same process after warm-up has already filled cache.
        _ = await _cachedStore.ListAsync<BookingDocument>(collection);

        var swWarm = Stopwatch.StartNew();
        var warmDocs = await _cachedStore.ListAsync<BookingDocument>(collection);
        swWarm.Stop();

        Assert.Equal(documentCount, warmDocs.Count);
        var warmMs = swWarm.Elapsed.TotalMilliseconds;

        var improvement = ((cleanStartAvg - warmMs) / cleanStartAvg) * 100;

        _output.WriteLine($"Clean-start average (first read after restart): {cleanStartAvg:F2}ms");
        _output.WriteLine($"Warm-start read (cache already primed):        {warmMs:F2}ms");
        _output.WriteLine($"Warm-start improvement:                         {improvement:F1}%\n");

        Assert.True(warmMs < cleanStartAvg, $"Warm start should be faster (warm={warmMs:F2}ms vs clean avg={cleanStartAvg:F2}ms)");
    }
}
