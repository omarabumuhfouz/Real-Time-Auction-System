namespace MazadZone.Application.Features.ChatAgent.DTOs;

/// <summary>
/// Lightweight DTO for serializing active auction context to the LLM.
/// Excludes heavy fields (images, descriptions, bids) to stay token-efficient.
/// </summary>
public sealed record ActiveAuctionContextDto(
    Guid Id,
    string Title,
    decimal CurrentBidAmount,
    DateTime EndTime,
    string Category);
