using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Users.Commands.ChangeEmail.Events;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.ChangeEmail.Events;

public class SendEmailChangedOldAddressAlertHandlerTests : UserBaseTest<SendEmailChangedOldAddressAlertHandler>
{
    [Fact]
    public async Task Handle_EmailChanged_SendsSecurityAlertToOldAddress()
    {
        // Arrange
        var userId = UserId.New();
        var oldEmail = Email.Create("original.owner@mazadzone.com").Value;
        var newEmail = Email.Create("attacker.or.new@mazadzone.com").Value;
        
        var domainEvent = new UserEmailChangedDomainEvent(userId, oldEmail, newEmail);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // ✅ Crucial Security Assertion: Verify the email went to the OLD address, not the new one!
        await _emailService.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(req => 
                req.To == oldEmail && 
                req.Subject == "⚠️ Alert: Your MazadZone email was changed" &&
                req.Body.Contains("did not authorize this change") &&
                req.Body.Contains("lock your account immediately")),
            Arg.Any<CancellationToken>());
    }
}