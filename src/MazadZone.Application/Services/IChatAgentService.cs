using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;

/// <summary>
/// Abstraction for the AI-powered sales chat agent.
/// The implementation resides in the Infrastructure layer and is auto-registered via Scrutor.
/// </summary>
public interface IChatAgentService : IScopedService
{
    /// <summary>
    /// Generates a sales response based on the user's message and the current active auctions context.
    /// </summary>
    /// <param name="userMessage">The user's chat message.</param>
    /// <param name="activeAuctionsJsonContext">A JSON string containing active auction data for RAG context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The AI-generated response string.</returns>
    Task<string> GenerateSalesResponseAsync(
        string userMessage,
        string activeAuctionsJsonContext,
        CancellationToken cancellationToken);
}
