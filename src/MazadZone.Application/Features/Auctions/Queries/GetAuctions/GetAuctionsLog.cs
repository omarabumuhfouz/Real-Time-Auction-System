using Microsoft.Extensions.Logging;

namespace MazadZone.Application.Features.Auctions.Queries.GetAuctions;

public static partial class GetAuctionsLog
{
    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetAuctionsAttempt, Level = LogLevel.Information, Message = "Handling GetAuctionsQuery with SearchTerm: {SearchTerm}, CategoryId: {CategoryId}, MinBidAmount: {MinBidAmount}, SortBy: {SortBy}, SortDirection: {SortDirection}.")]
    public static partial void LogHandlingGetAuctions(this ILogger logger, string? searchTerm, Guid? categoryId, decimal? minBidAmount, string? sortBy, string? sortDirection);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetAuctionsNoResults, Level = LogLevel.Warning, Message = "No auctions found matching the criteria: SearchTerm: {SearchTerm}, CategoryId: {CategoryId}, MinBidAmount: {MinBidAmount}, SortBy: {SortBy}, SortDirection: {SortDirection}.")]
    public static partial void LogNoAuctionsFound(this ILogger logger, string? searchTerm, Guid? categoryId, decimal? minBidAmount, string? sortBy, string? sortDirection);

    [LoggerMessage(EventId = MazadLogEvents.Auctions.GetAuctionsSuccess, Level = LogLevel.Information, Message = "Successfully retrieved {Count} auctions matching the criteria: SearchTerm: {SearchTerm}, CategoryId: {CategoryId}, MinBidAmount: {MinBidAmount}, SortBy: {SortBy}, SortDirection: {SortDirection}.")]
    public static partial void SuccessRetrievedAuctions(this ILogger logger, string? searchTerm, Guid? categoryId, decimal? minBidAmount, string? sortBy, string? sortDirection, int count);
}