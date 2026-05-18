using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.ChangePassword;

public class SendPasswordChangedEmailAlertHandlerTests : UserBaseTest<SendPasswordChangedEmailAlertHandler>
{
    [Fact]
    public async Task Handle_PasswordChanged_SendsSecurityAlertEmail()
    {
        // Arrange
        var userId = UserId.New();
        var targetEmail = Email.Create("secure.user@mazadzone.com").Value;
        
        // Build the event exactly as your User aggregate raises it
        var domainEvent = new UserPasswordChangedDomainEvent(userId, targetEmail);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        const string expectedTitle = "Security Update: Password Changed";

        // ✅ Verify the email service was called once, targeting the correct user with the exact text
        await _emailService.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(req => 
                req.To == targetEmail && 
                req.Subject == expectedTitle &&
                req.Body.Contains("password for your account was recently changed") &&
                req.Body.Contains("reset it immediately")),
            Arg.Any<CancellationToken>());
    }
}