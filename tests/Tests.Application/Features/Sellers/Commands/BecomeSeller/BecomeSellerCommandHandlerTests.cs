using MazadZone.Application.Features.Sellers.Commands.BecomeSeller;
using MazadZone.Domain.Users;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Sellers;
using MediatR;
using MazadZone.Domain.Auctions;

namespace Tests.Application.Features.Sellers.Commands.BecomeSeller;

public class BecomeSellerCommandHandlerTests : SellerBaseTest<BecomeSellerCommandHandler>
{
    [Fact]
    public async Task Handle_BaseUserDoesNotExist_ReturnsBidderNotFoundError()
    {
        // Arrange
        var command = SellerHelper.CreateBecomeSellerCommand();

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.NotFound);

        // Verify isolation: We aborted immediately without hitting other data dependencies
        await _bidderRepository.DidNotReceiveWithAnyArgs().GetNationalIdByBidderIdAsync(command.UserId, default);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_NationalIdDoesNotExistForBidder_ReturnsBidderNotFoundError()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        var command = SellerHelper.CreateBecomeSellerCommand() with {UserId = user.Id};

        _userRepository.GetByIdWithTokensAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _bidderRepository.GetNationalIdByBidderIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns((string?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.NotFound);

        // Verify we never created a seller aggregate or touched the unit of work
        _sellerRepository.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    

    [Fact]
    public async Task Handle_ValidCommand_CreatesSellerAndSavesChanges()
    {
        // Arrange
        var user = UserHelper.CreateActiveUser();
        var command = SellerHelper.CreateBecomeSellerCommand() with { UserId = user.Id };
        var expectedNationalId = "9991012345";

        _userRepository.GetByIdWithTokensAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        _bidderRepository.GetNationalIdByBidderIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(expectedNationalId);

        Seller capturedSeller = null!;
        _sellerRepository.When(x => x.Add(Arg.Any<Seller>()))
            .Do(callInfo => capturedSeller = callInfo.Arg<Seller>());

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());


        capturedSeller.ShouldNotBeNull();
        
        capturedSeller.Id.Value.ShouldBe(user.Id.Value); 
    }
}