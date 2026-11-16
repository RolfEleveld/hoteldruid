using System.Collections.Concurrent;
using System.Text;

namespace HotelDruid.Api.Services;

/// <summary>
/// Writes one log file per calendar day: api-yyyyMMdd.log.
/// A process that stays up past midnight moves to the new file on the next line.
/// </summary>
public sealed class DailyFileLoggerProvider : ILoggerProvider
{
    private readonly string _directory;
    private readonly ConcurrentDictionary<string, DailyFileLogger> _loggers = new(StringComparer.Ordinal);

    public DailyFileLoggerProvider(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(directory);
    }

    public ILogger CreateLogger(string categoryName)
        => _loggers.GetOrAdd(categoryName, name => new DailyFileLogger(_directory, name));

    public void Dispose()
    {
    }
}

public sealed class DailyFileLogger : ILogger
{
    private readonly string _directory;
    private readonly string _category;
    private readonly Lock _gate = new();
    private string _day = "";
    private StreamWriter? _writer;

    public DailyFileLogger(string directory, string category)
    {
        _directory = directory;
        _category = category;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel) || formatter is null)
            return;

        var message = formatter(state, exception);
        if (string.IsNullOrEmpty(message) && exception is null)
            return;

        var now = DateTimeOffset.Now;
        var line = new StringBuilder()
            .Append(now.ToString("O"))
            .Append(' ')
            .Append(logLevel)
            .Append(' ')
            .Append(_category)
            .Append(' ')
            .Append(message);
        if (exception is not null)
        {
            line.AppendLine();
            line.Append(exception);
        }

        lock (_gate)
        {
            var day = now.ToString("yyyyMMdd");
            if (_writer is null || _day != day)
            {
                _writer?.Dispose();
                var path = Path.Combine(_directory, $"api-{day}.log");
                _writer = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    AutoFlush = true
                };
                _day = day;
            }

            _writer.WriteLine(line.ToString());
        }
    }
}
