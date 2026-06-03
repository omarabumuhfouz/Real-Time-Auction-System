namespace Tests.Application.Features.DisputeTypes.Commands.Update;

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Features.DisputeTypes.Commands.Update;
using MazadZone.Domain.Disputes; 

public class UpdateDisputeTypeCommandHandlerTests : DisputeTypeBaseTest<UpdateDisputeTypeCommandHandler>
{
    [Fact]
    public async Task Handle_DisputeTypeNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new UpdateDisputeTypeCommand(DisputeTypeId.New(), "New Name", "New Description");

        // Mock repo to return null
        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns((DisputeType?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(DisputeTypeErrors.NotFound); 

        // Verify we aborted before saving
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_NameCreationFails_ReturnsDomainError()
    {
        // Arrange
        // We pass an empty name to force Name.Create() to fail
        var command = new UpdateDisputeTypeCommand(DisputeTypeId.New(), string.Empty, "Valid Description");
        
        var existingDisputeType = DisputeType.Create("Old Name", "Old Description").Value;

        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(existingDisputeType);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verify we aborted before saving
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        
        // Verify the entity was NOT mutated
        existingDisputeType.Name.Value.ShouldBe("Old Name");
    }

    [Fact]
    public async Task Handle_DescriptionCreationFails_ReturnsDomainError()
    {
        // Arrange
        // We pass an empty description to force Description.Create() to fail
        var command = new UpdateDisputeTypeCommand(DisputeTypeId.New(), "Valid Name", string.Empty);
        
        var existingDisputeType = DisputeType.Create("Old Name", "Old Description").Value;

        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(existingDisputeType);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verify we aborted before saving
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
        
        // Verify the entity was NOT mutated
        existingDisputeType.Description.Value.ShouldBe("Old Description");
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesEntityAndSavesChanges()
    {
        // Arrange
        var command = new UpdateDisputeTypeCommand(DisputeTypeId.New(), "Updated Name", "Updated Description");
        
        // Create the existing entity in its "old" state
        var existingDisputeType = DisputeType.Create("Old Name", "Old Description").Value;

        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(existingDisputeType);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // 💡 Verify the domain state actually mutated correctly (extracting primitive strings from the Value Objects)
        existingDisputeType.Name.Value.ShouldBe(command.Name);
        existingDisputeType.Description.Value.ShouldBe(command.Description);

        // Verify the infrastructure commit was called exactly once
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}