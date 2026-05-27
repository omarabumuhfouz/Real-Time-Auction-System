namespace MazadZone.Application.Features.ChatAgent.Commands.SendChatMessage;

/// <summary>
/// Command to send a chat message to the AI sales agent.
/// </summary>
public sealed record SendChatMessageCommand(
    Guid UserId,
    string UserMessage) : ICommand<string>;
