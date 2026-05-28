namespace Tests.Application.Features.DisputeTypes.Commands.Create;

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Features.DisputeTypes.Commands.Create;
using MazadZone.Domain.Disputes;

public class CreateDisputeTypeCommandHandlerTests : DisputeTypeBaseTest<CreateDisputeTypeCommandHandler>
{
    [Fact]
    public async Task Handle_DomainValidationFails_ReturnsFailureError()
    {
        // Arrange
        // Passing an empty name to force the DisputeType.Create domain factory to fail
        var command = new CreateDisputeTypeCommand(string.Empty, "This is an invalid dispute type because the name is empty.");

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        
        // Verify the process aborted before hitting the database
        _disputeTypeRepository.DidNotReceiveWithAnyArgs().Add(default!);
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesDisputeTypeAndSavesChanges()
    {
        // Arrange
        var command = DisputeTypeHelper.CreateValidCreateCommand();

        // Set Traps for Capture & Assert
        DisputeType capturedDisputeType = null!;
        
        // Ensure _repository is correctly typed as IDisputeTypeRepository in DisputeTypeBaseTest
        _disputeTypeRepository.When(x => x.Add(Arg.Any<DisputeType>()))
            .Do(info => capturedDisputeType = info.Arg<DisputeType>());

        // Act
        var result = await Handler.Handle(command, default);

        // Assert - Flow and Response
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);

        // Assert - Data Integrity (Verify Traps)
        capturedDisputeType.ShouldNotBeNull();
        
        // 💡 FIX: Appending .Value to unwrap the Value Object into a primitive string for ShouldBe()
        // If your Value Object property is named something else (like .Text), use that instead.
        capturedDisputeType.Name.Value.ShouldBe(command.Name);
        capturedDisputeType.Description.Value.ShouldBe(command.Description);
        
        // The returned ID should match the newly created entity's ID
        result.Value.ShouldBe(capturedDisputeType.Id.Value); 

        // Verify Infrastructure commits
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}