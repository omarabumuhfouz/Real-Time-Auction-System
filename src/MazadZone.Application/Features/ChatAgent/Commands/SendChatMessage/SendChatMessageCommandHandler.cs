using System.Text.Json;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.ChatAgent.Commands.SendChatMessage;

/// <summary>
/// Handles the SendChatMessageCommand by retrieving active auctions,
/// serializing them as context, and passing the user message to the AI service.
/// </summary>
public sealed class SendChatMessageCommandHandler(
    IAuctionQueries _auctionQueries,
    IChatAgentService _chatAgentService,
    ILogger<SendChatMessageCommandHandler> _logger
) : ICommandHandler<SendChatMessageCommand, string>
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<Result<string>> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing chat message for user {UserId}", request.UserId);

        // 1. Retrieve active auctions for RAG context
        var activeAuctions = await _auctionQueries.GetActiveAuctionContextAsync(cancellationToken);

        // 2. Serialize to a lightweight JSON string (token-efficient)
        var auctionsJson = JsonSerializer.Serialize(activeAuctions, _jsonOptions);

        _logger.LogDebug("Active auctions context contains {Count} auctions", activeAuctions.Count);

        // 3. Pass user message and context to the AI service
        var aiResponse = await _chatAgentService.GenerateSalesResponseAsync(
            request.UserMessage,
            auctionsJson,
            cancellationToken);

        return Result.Success(aiResponse);
    }
}
