namespace Tests.Application.Features.DisputeTypes.Commands.Delete;

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Features.DisputeTypes.Commands.Delete;
using MazadZone.Domain.Disputes;

public class DeleteDisputeTypeCommandHandlerTests : DisputeTypeBaseTest<DeleteDisputeTypeCommandHandler>
{
    [Fact]
    public async Task Handle_DisputeTypeNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new DeleteDisputeTypeCommand(DisputeTypeId.New());

        // Mock the repository to return null (not found)
        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns((DisputeType?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(DisputeTypeErrors.NotFound); // Assumes DisputeTypeErrors.NotFound exists

        // Verify we aborted before saving
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_AlreadyDeleted_ReturnsIdempotentSuccess()
    {
        // Arrange
        var command = new DeleteDisputeTypeCommand(DisputeTypeId.New());
        
        // Create a valid entity using your factory
        var disputeType = DisputeType.Create("Damaged Item", "Item arrived broken").Value;
        
        // 💡 Pre-delete it to set IsActive to false
        disputeType.Delete(); 

        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(disputeType);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesEntityAndSavesChanges()
    {
        // Arrange
        var command = new DeleteDisputeTypeCommand(DisputeTypeId.New());
        
        // Create a fresh, active entity
        var disputeType = DisputeType.Create("Missing Parts", "Box was missing cables").Value;

        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(disputeType);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify the infrastructure commit was called
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}