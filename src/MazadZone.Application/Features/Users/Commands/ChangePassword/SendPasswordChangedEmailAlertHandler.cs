using MazadZone.Application.Common.Interfaces;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.ChangePassword;

public class SendPasswordChangedEmailAlertHandler(IEmailService emailService) : INotificationHandler<UserPasswordChangedDomainEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(UserPasswordChangedDomainEvent notification, CancellationToken ct)
    {
        const string title = "Security Update: Password Changed";

        await _emailService.SendEmailAsync(new EmailRequest(
            To: notification.CurrentEmail,
            Subject: title,
            Body: "The password for your account was recently changed. If this wasn't you, reset it immediately."
        ), ct);
    }
}