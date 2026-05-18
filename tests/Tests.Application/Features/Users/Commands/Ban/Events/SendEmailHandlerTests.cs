using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Users.Commands.Ban.Events;
using MazadZone.Domain.Users.Events;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Users.Commands.Ban.Events;

public class SendEmailHandlerTests : UserBaseTest<SendEmailHandler>
{
    [Fact]
    public async Task Handle_UserBanned_SendsTerminationEmail()
    {
        // Arrange
        var targetEmail = "violator@mazadzone.com";
        var banReason = "Fraudulent bidding activity.";
        
        // We generate a dummy UserId because the email handler doesn't strictly need it, 
        // but the event constructor requires it.
        var domainEvent = new UserBannedDomainEvent(UserId.New(), banReason, targetEmail);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // Verify the email service was called exactly once with the correctly mapped DTO
        await _emailService.Received(1).SendEmailAsync(
            Arg.Is<EmailRequest>(req => 
                req.To == targetEmail &&
                req.Subject == "Account Terminated: MazadZone" &&
                req.Body.Contains(banReason) &&
                req.IsHtml == false),
            Arg.Any<CancellationToken>());
    }
}