using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Users.Commands.Activate;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Activate;

public class SendUserActivatedEmailHandlerTests : UserBaseTest<SendUserActivatedEmailHandler>
{
    [Fact]
    public async Task Handle_UserActivated_SendsActivationEmail()
    {
        // Arrange
        const string targetEmail = "omar.developer@gmail.com";
        var domainEvent = new UserActivatedDomainEvent(UserId.New(), targetEmail);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // ✅ Verify that SendEmailAsync was called exactly once with the expected string mappings
        await _emailService.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(req => 
                req.To == targetEmail &&
                req.Subject == "Welcome Back! Account Activated" &&
                req.Body.Contains("Your account suspension has been lifted") &&
                req.Body.Contains("auctions cancelled during your suspension")),
            Arg.Any<CancellationToken>());
    }
}