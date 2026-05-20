using MazadZone.Application.Features.Bidders.Commands.Register;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users;
using MazadZone.Domain.Users.ValueObjects;

namespace Tests.Application.Features.Bidders.Commands.Register;

public class RegisterBidderCommandHandlerTests : BidderBaseTest<RegisterBidderCommandHandler>
{
    [Fact]
    public async Task Handle_EmailIsInvalid_ReturnsFailureError()
    {
        // Arrange
        var command = BidderHelper.CreateValidRegisterCommand() with { Email = "invalid-email" };

        // Act
        // Must cast to the interface because the handler uses explicit interface implementation
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verifies the process aborted before hitting the DB
        await _userRepository.DidNotReceiveWithAnyArgs().IsEmailInUseAsync(default!, default);
    }

    [Fact]
    public async Task Handle_EmailAlreadyInUse_ReturnsEmailAlreadyInUseError()
    {
        // Arrange
        var command = BidderHelper.CreateValidRegisterCommand();

        // We use Arg.Any to catch whatever parsed email object is passed to the repo
        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(EmailErrors.AlreadyInUse); // Assuming EmailErrors lives here

        _userRepository.DidNotReceiveWithAnyArgs().Add(default!);
    }

    [Fact]
    public async Task Handle_UserCreationFails_ReturnsDomainError()
    {
        // Arrange
        // Passing an empty first name to force the User.Create domain factory to fail
        var command = BidderHelper.CreateValidRegisterCommand() with { FirstName = string.Empty };

        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommand_RegistersBidderAndSavesChanges()
    {
        // Arrange
        var command = BidderHelper.CreateValidRegisterCommand();
        var expectedPasswordHash = "hashed_password_123";
        var dummyAddress = Address.Create("Jordan", "Amman", "Queen Rania St", "11118").Value;

        _userRepository.IsEmailInUseAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>()).Returns(false);
        _passwordService.HashPassword(command.Password).Returns(expectedPasswordHash);
        
        // Setup Tokens
        _tokenProvider.GenerateAccessToken(Arg.Any<User>()).Returns("mock_access_token");
        _tokenProvider.GenerateRefreshToken().Returns("mock_refresh_token");
        _tokenProvider.HashToken("mock_refresh_token").Returns("mock_hashed_refresh");

        // Set Traps for Capture & Assert
        User capturedUser = null!;
        Bidder capturedBidder = null!;
        
        _userRepository.When(x => x.Add(Arg.Any<User>()))
            .Do(info => capturedUser = info.Arg<User>());
            
        _bidderRepository.When(x => x.Add(Arg.Any<Bidder>()))
            .Do(info => capturedBidder = info.Arg<Bidder>());

        // Act
        var result = await Handler.Handle(command, default);

        // Assert - Flow and Response
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TokenInfo.Token.ShouldBe("mock_access_token");
        result.Value.TokenInfo.RefreshToken.ShouldBe("mock_refresh_token");
        result.Value.ProfileInfo.Email.ShouldBe(command.Email);
        result.Value.ProfileInfo.NationalId.ShouldBe(command.NationalId);

        // Assert - Data Integrity (Verify Traps)
        capturedUser.ShouldNotBeNull();
        capturedUser.Email.Value.ShouldBe(command.Email);
        capturedUser.PasswordHash.Value.ShouldBe(expectedPasswordHash); // Verifies hash was assigned, not plaintext
        capturedUser.Roles.ShouldContain(UserRole.Bidder);

        capturedBidder.ShouldNotBeNull();
        capturedBidder.Id.Value.ShouldBe(capturedUser.Id.Value); // Bidder ID must perfectly match User ID
        capturedBidder.NationalId.ShouldBe(command.NationalId);
        capturedBidder.DefaultShippingAddress.ShouldBe(dummyAddress);

        // Verify Infrastructure commits
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}