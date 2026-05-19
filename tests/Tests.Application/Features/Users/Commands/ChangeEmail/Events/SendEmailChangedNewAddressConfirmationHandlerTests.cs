using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Users.Commands.ChangeEmail.Events;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.ChangeEmail.Events;

public class SendEmailChangedNewAddressConfirmationHandlerTests : UserBaseTest<SendEmailChangedNewAddressConfirmationHandler>
{
    [Fact]
    public async Task Handle_EmailChanged_SendsConfirmationToNewAddress()
    {
        // Arrange
        var userId = UserId.New();
        var oldEmail = Email.Create("old.email@mazadzone.com").Value;
        var newEmail = Email.Create("new.email@mazadzone.com").Value;
        
        var domainEvent = new UserEmailChangedDomainEvent(userId, oldEmail, newEmail);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // ✅ Verify that SendEmailAsync was called once, pointing exactly to the NEW email address
        await _emailService.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(req => 
                req.To == newEmail && // Crucial boundary check: must go to the new address!
                req.Subject == "Confirmation: New Email Address" &&
                req.Body.Contains("successfully updated your email") &&
                req.Body.Contains("future logins")),
            Arg.Any<CancellationToken>());
    }
}