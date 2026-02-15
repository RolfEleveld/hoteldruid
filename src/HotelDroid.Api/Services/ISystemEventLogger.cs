namespace HotelDroid.Api.Services;

/// <summary>
/// Structured event logging for system operations and security events.
/// Integrates with Windows Event Log and supports SIEM consumption.
/// 
/// Event IDs by category:
/// 1000-1099: System initialization and health
/// 2000-2099: Authentication and authorization
/// 3000-3099: Data operations (CRUD)
/// 4000-4099: Ledger and transactions
/// 5000-5099: Errors and exceptions
/// </summary>
public interface ISystemEventLogger
{
    /// <summary>
    /// Log a data operation (create, read, update, delete).
    /// </summary>
    void LogDataOperation(
        string operationType,      // "CREATE", "READ", "UPDATE", "DELETE"
        string collectionName,
        string documentId,
        string? documentName = null,
        string? userName = null,
        string? details = null);

    /// <summary>
    /// Log a successful operation.
    /// </summary>
    void LogSuccess(
        string operation,
        string? details = null,
        string? userName = null);

    /// <summary>
    /// Log an error or exception.
    /// </summary>
    void LogError(
        string operation,
        string message,
        Exception? exception = null,
        string? details = null);

    /// <summary>
    /// Log a security-related event.
    /// </summary>
    void LogSecurityEvent(
        string eventType,          // "UNAUTHORIZED", "INVALID_INPUT", "PATH_TRAVERSAL", etc.
        string? details = null,
        string? userName = null,
        string? sourceIp = null);

    /// <summary>
    /// Log a warning condition.
    /// </summary>
    void LogWarning(
        string operation,
        string message,
        string? details = null);

    /// <summary>
    /// Log ledger/transaction event.
    /// </summary>
    void LogLedgerEvent(
        string eventType,          // "SNAPSHOT", "TRANSACTION", "CLOSURE", etc.
        string entityId,
        decimal amount,
        string? details = null,
        string? userName = null);
}

/// <summary>
/// Windows Event Log implementation with SIEM-compatible structured events.
/// </summary>
public class WindowsEventLogger : ISystemEventLogger
{
    private readonly ILogger<WindowsEventLogger> _logger;
    private const string LogSourceName = "HotelDroid";
    private const string LogName = "Application";

    public WindowsEventLogger(ILogger<WindowsEventLogger> logger)
    {
        _logger = logger;
    }

    public void LogDataOperation(
        string operationType,
        string collectionName,
        string documentId,
        string? documentName = null,
        string? userName = null,
        string? details = null)
    {
        var eventId = (operationType) switch
        {
            "CREATE" => 3001,
            "READ" => 3002,
            "UPDATE" => 3003,
            "DELETE" => 3004,
            _ => 3000
        };

        var message = FormatStructuredEvent(new
        {
            EventType = "DATA_OPERATION",
            OperationType = operationType,
            Collection = collectionName,
            DocumentId = documentId,
            DocumentName = documentName,
            User = userName ?? "system",
            Timestamp = DateTime.UtcNow.ToString("O"),
            Details = details
        });

        _logger.LogInformation("{Message}", message);
    }

    public void LogSuccess(
        string operation,
        string? details = null,
        string? userName = null)
    {
        var message = FormatStructuredEvent(new
        {
            EventType = "SUCCESS",
            Operation = operation,
            User = userName ?? "system",
            Timestamp = DateTime.UtcNow.ToString("O"),
            Details = details
        });

        _logger.LogInformation("{Message}", message);
    }

    public void LogError(
        string operation,
        string message,
        Exception? exception = null,
        string? details = null)
    {
        var eventMessage = FormatStructuredEvent(new
        {
            EventType = "ERROR",
            EventId = 5000,
            Operation = operation,
            Message = message,
            ExceptionType = exception?.GetType().Name,
            ExceptionMessage = exception?.Message,
            StackTrace = exception?.StackTrace,
            Timestamp = DateTime.UtcNow.ToString("O"),
            Details = details
        });

        _logger.LogError("{Message}", eventMessage);
    }

    public void LogSecurityEvent(
        string eventType,
        string? details = null,
        string? userName = null,
        string? sourceIp = null)
    {
        var eventId = eventType switch
        {
            "UNAUTHORIZED" => 2001,
            "INVALID_INPUT" => 2002,
            "PATH_TRAVERSAL" => 2003,
            "AUTHENTICATION_FAILED" => 2004,
            _ => 2000
        };

        var message = FormatStructuredEvent(new
        {
            EventType = "SECURITY",
            EventId = eventId,
            SecurityEvent = eventType,
            User = userName ?? "unknown",
            SourceIP = sourceIp ?? "unknown",
            Timestamp = DateTime.UtcNow.ToString("O"),
            Details = details,
            Severity = "WARNING"
        });

        _logger.LogWarning("{Message}", message);
    }

    public void LogWarning(
        string operation,
        string message,
        string? details = null)
    {
        var eventMessage = FormatStructuredEvent(new
        {
            EventType = "WARNING",
            Operation = operation,
            Message = message,
            Timestamp = DateTime.UtcNow.ToString("O"),
            Details = details
        });

        _logger.LogWarning("{Message}", eventMessage);
    }

    public void LogLedgerEvent(
        string eventType,
        string entityId,
        decimal amount,
        string? details = null,
        string? userName = null)
    {
        var eventId = eventType switch
        {
            "SNAPSHOT" => 4001,
            "TRANSACTION" => 4002,
            "CLOSURE" => 4003,
            "RECONCILIATION" => 4004,
            _ => 4000
        };

        var message = FormatStructuredEvent(new
        {
            EventType = "LEDGER",
            EventId = eventId,
            LedgerEvent = eventType,
            EntityId = entityId,
            Amount = amount,
            User = userName ?? "system",
            Timestamp = DateTime.UtcNow.ToString("O"),
            Details = details
        });

        _logger.LogInformation("{Message}", message);
    }

    private static string FormatStructuredEvent(object eventData)
    {
        // Format as JSON for SIEM compatibility
        return System.Text.Json.JsonSerializer.Serialize(
            eventData,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = false });
    }
}
