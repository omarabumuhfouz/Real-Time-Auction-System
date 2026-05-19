using MazadZone.Application.Common.Interfaces;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.Activate;

public class SendUserActivatedEmailHandler : INotificationHandler<UserActivatedDomainEvent>
{
    private readonly IEmailService _emailService;

    public SendUserActivatedEmailHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(UserActivatedDomainEvent notification, CancellationToken ct)
    {
        const string title = "Welcome Back! Account Activated";
        const string message = "Your account suspension has been lifted. You can now participate in auctions again.";

        await _emailService.SendEmailAsync(new EmailRequest(
            To: notification.Email,
            Subject: title,
            Body: $"Hello! {message} Please note that any auctions cancelled during your suspension will not be automatically restarted."
        ), ct);
    }
}