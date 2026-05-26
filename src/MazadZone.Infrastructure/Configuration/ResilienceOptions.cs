namespace MazadZone.Infrastructure.Configuration;

public sealed class ResilienceOptions
{
    public const string SectionName = "Resilience";

    public int RetryCount { get; init; } = 3;
    public int BaseDelaySeconds { get; init; } = 1;
    public int MaxDelaySeconds { get; init; } = 1;

// New Bulkhead Properties
    public int BulkheadMaxConcurrentRequests { get; set; } = 10;
    public int BulkheadMaxQueuedRequests { get; set; } = 5;

}