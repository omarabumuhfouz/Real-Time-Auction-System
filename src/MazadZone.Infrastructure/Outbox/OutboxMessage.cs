namespace MazadZone.Infrastructure.Outbox;

using System;

public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    // The fully qualified assembly type name (so we know how to deserialize it later)
    public string Type { get; init; } = string.Empty;
    
    // The actual event data serialized as JSON
    public string Content { get; init; } = string.Empty;
    
    public DateTime OccurredOnUtc { get; init; }
    
    // Null means it hasn't been processed yet
    public DateTime? ProcessedOnUtc { get; set; }
    
    // Useful for tracking if a specific event keeps failing
    public string? Error { get; set; } 
}