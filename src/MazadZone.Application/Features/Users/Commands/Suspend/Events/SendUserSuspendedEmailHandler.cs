using MazadZone.Application.Common.Interfaces;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.Suspend.Events;

public class SendUserSuspendedEmailHandler(
    IEmailService emailService) : INotificationHandler<UserSuspendedDomainEvent>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(UserSuspendedDomainEvent notification, CancellationToken ct)
    {
        var title = "Account Suspended";
        var message = $"Your account has been suspended due to: {notification.Reason}. " +
                      $"Estimated return: {notification.ReinstatementDate?.ToShortDateString() ?? "Indefinite"}";

        var emailRequest = new EmailRequest(
            To: notification.Email,
            Subject: title,
            Body: message,
            IsHtml: false
        );

        await _emailService.SendEmailAsync(emailRequest, ct);
    }
}