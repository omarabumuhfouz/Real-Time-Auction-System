namespace MazadZone.Application.Features.Emails.Commands.NotifyUser;

public sealed record NotifyUserCommand(
    UserId UserId, 
    string Title, 
    string Message) : ICommand<Unit>;