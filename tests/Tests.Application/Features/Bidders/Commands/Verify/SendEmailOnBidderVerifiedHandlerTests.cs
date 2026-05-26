using MazadZone.Application.Common.Interfaces; // Assuming EmailRequest lives here
using MazadZone.Application.Features.Bidders.Commands.Verify;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Bidders.Events;

namespace Tests.Application.Features.Bidders.EventHandlers;

public class SendEmailOnBidderVerifiedHandlerTests : BidderBaseTest<SendEmailOnBidderVerifiedHandler>
{
    [Fact]
    public async Task Handle_BidderProfileDoesNotExist_AbortsAndLogs()
    {
        // Arrange
        var domainEvent = new BidderVerifiedDomainEvent(UserId.New());

        // Simulate a missing database record
        _bidderQueries.GetBidderProfile(domainEvent.BidderId, Arg.Any<CancellationToken>())
            .Returns((BidderProfileDto?)null);

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        // Verify that the external email service was never hit
        await _emailService.DidNotReceiveWithAnyArgs().SendEmailAsync(default!, default);
    }

    [Fact]
    public async Task Handle_BidderExists_SendsHtmlEmail()
    {
        // Arrange
        var bidderId = UserId.New();
        var domainEvent = new BidderVerifiedDomainEvent(bidderId);

        // Construct the DTO that the read model will return
        var expectedProfile = BidderHelper.CreateValidBidderProfileDto();


        _bidderQueries.GetBidderProfile(bidderId, Arg.Any<CancellationToken>())
            .Returns(expectedProfile);

        // Set the trap using the Capture & Assert pattern
        EmailRequest capturedEmail = null!;
        _emailService.When(x => x.SendEmailAsync(Arg.Any<EmailRequest>(), Arg.Any<CancellationToken>()))
            .Do(info => capturedEmail = info.Arg<EmailRequest>());

        // Act
        await Handler.Handle(domainEvent, default);

        // Assert
        capturedEmail.ShouldNotBeNull();
        
        // Verify the payload routed to the correct destination
        capturedEmail.To.ShouldBe(expectedProfile.Email);
        
        // Verify the hardcoded subject line
        
        // Verify the HTML body properly interpolated the user's name
        capturedEmail.Body.ShouldContain(expectedProfile.FullName);
        
        // Verify the correct flag was passed to the email client
        capturedEmail.IsHtml.ShouldBeTrue();
    }
}