using MazadZone.Application.Features.Bidders.Commands.Verify;
using MazadZone.Domain.Bidders;
using MediatR;

namespace Tests.Application.Features.Bidders.Commands.Verify;

public class VerifiyBidderCommandHandlerTests : BidderBaseTest<VerifyBidderCommandHandler>
{
    [Fact]
    public async Task Handle_BidderDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = BidderHelper.CreateVerifyBidderCommand();

        _bidderRepository.GetByIdAsync(command.BidderId.Value, Arg.Any<CancellationToken>())
            .Returns((Bidder?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.NotFound);

        // Verify isolation: We short-circuited before attempting to save
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_BidderExists_VerifiesBidderAndSavesChanges()
    {
        // Arrange
        var command = BidderHelper.CreateVerifyBidderCommand();
        var bidder = BidderHelper.CreateUnverifiedBidder(command.BidderId);

        _bidderRepository.GetByIdAsync(command.BidderId.Value, Arg.Any<CancellationToken>())
            .Returns(bidder);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);

        // Verify internal aggregate state mutation took place
        // (Assuming IsVerified is a public boolean on your Bidder aggregate)
        bidder.IsVerified.ShouldBeTrue();

        // Verify infrastructure triggers were dispatched
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}