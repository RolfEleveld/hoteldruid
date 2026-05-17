namespace HotelDruid.Api.Services;

public interface ICacheWarmupState
{
    bool IsEnabled { get; }
    bool IsInProgress { get; }
    bool IsCompleted { get; }
    DateTimeOffset? StartedAtUtc { get; }
    DateTimeOffset? CompletedAtUtc { get; }
    int CollectionsProcessed { get; }
    int DocumentsWarmed { get; }
    string? LastError { get; }
}

public sealed class CacheWarmupState : ICacheWarmupState
{
    private readonly object _gate = new();

    public bool IsEnabled { get; private set; }
    public bool IsInProgress { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTimeOffset? StartedAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public int CollectionsProcessed { get; private set; }
    public int DocumentsWarmed { get; private set; }
    public string? LastError { get; private set; }

    public void MarkSkipped()
    {
        lock (_gate)
        {
            IsEnabled = false;
            IsInProgress = false;
            IsCompleted = true;
            StartedAtUtc = DateTimeOffset.UtcNow;
            CompletedAtUtc = DateTimeOffset.UtcNow;
            LastError = null;
        }
    }

    public void MarkStarted()
    {
        lock (_gate)
        {
            IsEnabled = true;
            IsInProgress = true;
            IsCompleted = false;
            StartedAtUtc = DateTimeOffset.UtcNow;
            CompletedAtUtc = null;
            CollectionsProcessed = 0;
            DocumentsWarmed = 0;
            LastError = null;
        }
    }

    public void AddCollectionResult(int documentsWarmed)
    {
        lock (_gate)
        {
            CollectionsProcessed++;
            DocumentsWarmed += documentsWarmed;
        }
    }

    public void MarkCompleted()
    {
        lock (_gate)
        {
            IsInProgress = false;
            IsCompleted = true;
            CompletedAtUtc = DateTimeOffset.UtcNow;
        }
    }

    public void MarkFailed(Exception ex)
    {
        lock (_gate)
        {
            IsInProgress = false;
            IsCompleted = true;
            CompletedAtUtc = DateTimeOffset.UtcNow;
            LastError = ex.Message;
        }
    }
}
