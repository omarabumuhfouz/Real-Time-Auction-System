using MazadZone.Application.Common.Interfaces;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.ChangeEmail.Events;

public class SendEmailChangedOldAddressAlertHandler(IEmailService emailService) : INotificationHandler<UserEmailChangedDomainEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(UserEmailChangedDomainEvent notification, CancellationToken ct)
    {
        await _emailService.SendEmailAsync(new EmailRequest(
            To: notification.OldEmail,
            Subject: "⚠️ Alert: Your MazadZone email was changed",
            Body: "If you did not authorize this change, please lock your account immediately."
        ), ct);
    }
}