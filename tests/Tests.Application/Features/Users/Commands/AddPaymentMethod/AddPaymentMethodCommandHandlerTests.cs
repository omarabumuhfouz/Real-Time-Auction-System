namespace Tests.Application.Features.Users.Commands.AddPaymentMethod;

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Commands.AddPaymentMethod;
using MazadZone.Domain.Users.Errors;
using MazadZone.Domain.Users.Enums;
using MazadZone.Domain.Users;

// using MazadZone.Domain.Users.ValueObjects; // Where UserId lives

public class AddPaymentMethodCommandHandlerTests : UserBaseTest<AddPaymentMethodCommandHandler>
{
    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new AddPaymentMethodCommand(
            UserId.New(), "1234", 12, 2030, "John Doe", CardBrand.Visa, "tok_123", true);

        // Mock repo to return null
        _userRepository.GetByIdWithPaymentMethodsAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(UserErrors.NotFound);

        // Verify we aborted before saving
        _userRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_DomainValidationFails_ReturnsDomainError()
    {
        // Arrange
        var user = User.Create("john@example.com", "Password123!", "07912345678", "John", "Doe","Thrid Nmae", "Last Name", UserRole.Bidder).Value; 
        
        // Pass invalid data (month 13) to force the domain method to fail.
        var command = new AddPaymentMethodCommand(
            user.Id, "1234", 13, 2030, "John Doe", CardBrand.Mastercard, string.Empty, true); 

        _userRepository.GetByIdWithPaymentMethodsAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verify we aborted before mutating database state
        _userRepository.DidNotReceiveWithAnyArgs().Update(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsPaymentMethodAndSavesChanges()
    {
        // Arrange
        var user = User.Create("john@example.com", "Password123!", "07912345678", "John", "Doe","Thrid Nmae", "Last Name", UserRole.Bidder).Value; 
        
        var command = new AddPaymentMethodCommand(
            user.Id, "4242", 12, 2030, "John Doe", CardBrand.Visa, "tok_valid123", true);

        _userRepository.GetByIdWithPaymentMethodsAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        
        result.Value.Last4Digits.ShouldBe(command.Last4Digits);
        result.Value.Brand.ShouldBe(command.Brand);
        result.Value.IsDefault.ShouldBeTrue();

        _userRepository.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}