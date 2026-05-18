using MazadZone.Application.Common.Interfaces;
using MazadZone.Domain.Users.Events;

namespace MazadZone.Application.Features.Users.Commands.ChangeEmail.Events;

public class SendEmailChangedNewAddressConfirmationHandler : INotificationHandler<UserEmailChangedDomainEvent>
{
    private readonly IEmailService _emailService;

    public SendEmailChangedNewAddressConfirmationHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(UserEmailChangedDomainEvent notification, CancellationToken ct)
    {
        await _emailService.SendEmailAsync(new EmailRequest(
            To: notification.NewEmail,
            Subject: "Confirmation: New Email Address",
            Body: "You have successfully updated your email. Use this address for all future logins."
        ), ct);
    }
}