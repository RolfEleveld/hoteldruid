namespace HotelDruid.Api.Services.Tests;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

/// <summary>
/// Test fixtures and helpers for setting up test environments with proper DI and logging.
/// </summary>
public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }
    public ILogger<T> GetLogger<T>() => ServiceProvider.GetRequiredService<ILogger<T>>();

    public TestFixture()
    {
        var services = new ServiceCollection();

        // Add logging with console output
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Add event logging
        services.AddSingleton<ISystemEventLogger, WindowsEventLogger>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        (ServiceProvider as IDisposable)?.Dispose();
    }
}

/// <summary>
/// Test output logger adapter for xUnit integration.
/// Bridges xUnit ITestOutputHelper with ILogger.
/// </summary>
public class XunitLogger : ILogger
{
    private readonly ITestOutputHelper _output;
    private readonly string _categoryName;

    public XunitLogger(ITestOutputHelper output, string categoryName)
    {
        _output = output;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Debug;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        var timestamp = DateTime.UtcNow.ToString("O");
        var prefix = logLevel switch
        {
            LogLevel.Trace => "[TRC]",
            LogLevel.Debug => "[DBG]",
            LogLevel.Information => "[INF]",
            LogLevel.Warning => "[WRN]",
            LogLevel.Error => "[ERR]",
            LogLevel.Critical => "[CRT]",
            _ => "[???]"
        };

        var logMessage = $"{timestamp} {prefix} [{_categoryName}] {message}";
        
        _output.WriteLine(logMessage);

        if (exception != null)
        {
            _output.WriteLine($"Exception: {exception}");
        }
    }
}

/// <summary>
/// Logger provider for xUnit tests.
/// </summary>
public class XunitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _output;

    public XunitLoggerProvider(ITestOutputHelper output)
    {
        _output = output;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XunitLogger(_output, categoryName);
    }

    public void Dispose()
    {
    }
}

/// <summary>
/// Test event tracking for assertions.
/// </summary>
public class TestEventCapture : ISystemEventLogger
{
    public List<EventRecord> Events { get; } = new();

    public void LogDataOperation(string operationType, string collectionName, string documentId, string? documentName = null, string? userName = null, string? details = null)
    {
        Events.Add(new EventRecord
        {
            Timestamp = DateTime.UtcNow,
            Type = "DATA_OPERATION",
            OperationType = operationType,
            CollectionName = collectionName,
            DocumentId = documentId,
            DocumentName = documentName,
            UserName = userName,
            Details = details
        });
    }

    public void LogSuccess(string operation, string? details = null, string? userName = null)
    {
        Events.Add(new EventRecord
        {
            Timestamp = DateTime.UtcNow,
            Type = "SUCCESS",
            Operation = operation,
            Details = details,
            UserName = userName
        });
    }

    public void LogError(string operation, string message, Exception? exception = null, string? details = null)
    {
        Events.Add(new EventRecord
        {
            Timestamp = DateTime.UtcNow,
            Type = "ERROR",
            Operation = operation,
            Message = message,
            ExceptionType = exception?.GetType().Name,
            Details = details
        });
    }

    public void LogSecurityEvent(string eventType, string? details = null, string? userName = null, string? sourceIp = null)
    {
        Events.Add(new EventRecord
        {
            Timestamp = DateTime.UtcNow,
            Type = "SECURITY",
            SecurityEventType = eventType,
            UserName = userName,
            SourceIP = sourceIp,
            Details = details
        });
    }

    public void LogWarning(string operation, string message, string? details = null)
    {
        Events.Add(new EventRecord
        {
            Timestamp = DateTime.UtcNow,
            Type = "WARNING",
            Operation = operation,
            Message = message,
            Details = details
        });
    }

    public void LogLedgerEvent(string eventType, string entityId, decimal amount, string? details = null, string? userName = null)
    {
        Events.Add(new EventRecord
        {
            Timestamp = DateTime.UtcNow,
            Type = "LEDGER",
            LedgerEventType = eventType,
            EntityId = entityId,
            Amount = amount,
            Details = details,
            UserName = userName
        });
    }

    public class EventRecord
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Operation { get; set; }
        public string? OperationType { get; set; }
        public string? CollectionName { get; set; }
        public string? DocumentId { get; set; }
        public string? DocumentName { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
        public string? ExceptionType { get; set; }
        public string? SecurityEventType { get; set; }
        public string? SourceIP { get; set; }
        public string? LedgerEventType { get; set; }
        public string? EntityId { get; set; }
        public decimal Amount { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:O}] {Type}: {Operation ?? OperationType ?? SecurityEventType ?? LedgerEventType ?? ""} | {Message ?? Details ?? ""}";
        }
    }
}

/// <summary>
/// Environment setup helper for tests.
/// </summary>
public class TestEnvironment
{
    private string _tempDirectory = null!;

    public string TempDirectory => _tempDirectory;

    public void Setup()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"HotelDruid-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Cleanup()
    {
        if (Directory.Exists(_tempDirectory))
        {
            try
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
            catch
            {
                // Best effort
            }
        }
    }

    public string GetCollectionPath(string collectionName)
    {
        return Path.Combine(_tempDirectory, "collections", collectionName);
    }

    public List<string> GetAllJsonFiles(string collectionName)
    {
        var path = GetCollectionPath(collectionName);
        if (!Directory.Exists(path))
            return new List<string>();

        return Directory.GetFiles(path, "*.json").ToList();
    }

    public string ReadFile(string collectionName, string fileName)
    {
        var filePath = Path.Combine(GetCollectionPath(collectionName), fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return File.ReadAllText(filePath);
    }

    public void WriteFile(string collectionName, string fileName, string content)
    {
        var dirPath = GetCollectionPath(collectionName);
        Directory.CreateDirectory(dirPath);
        
        var filePath = Path.Combine(dirPath, fileName);
        File.WriteAllText(filePath, content);
    }

    public bool FileExists(string collectionName, string fileName)
    {
        var filePath = Path.Combine(GetCollectionPath(collectionName), fileName);
        return File.Exists(filePath);
    }

    public long GetFileSize(string collectionName, string fileName)
    {
        var filePath = Path.Combine(GetCollectionPath(collectionName), fileName);
        if (!File.Exists(filePath))
            return 0;

        return new FileInfo(filePath).Length;
    }
}

