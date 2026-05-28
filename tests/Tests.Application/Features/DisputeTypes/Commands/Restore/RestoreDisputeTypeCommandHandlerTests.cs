namespace Tests.Application.Features.DisputeTypes.Commands.Restore;

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Features.DisputeTypes.Commands.Restore;
using MazadZone.Domain.Disputes;

public class RestoreDisputeTypeCommandHandlerTests : DisputeTypeBaseTest<RestoreDisputeTypeCommandHandler>
{
    [Fact]
    public async Task Handle_DisputeTypeNotFound_ReturnsNotFoundError()
    {
        // Arrange
        var command = new RestoreDisputeTypeCommand(DisputeTypeId.New());

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
    public async Task Handle_AlreadyActive_ReturnsIdempotentSuccessWithoutSaving()
    {
        // Arrange
        var command = new RestoreDisputeTypeCommand(DisputeTypeId.New());

        // 💡 Create a fresh entity. By default in your factory, IsActive is true.
        var disputeType = DisputeType.Create("Delayed Shipping", "Seller did not ship on time").Value;

        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(disputeType);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // 💡 Because of your Application-level idempotency check (if (disputeType.IsActive) return Unit.Value;),
        // we guarantee the database is never hit with an unnecessary save operation.
        await _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_ValidCommand_RestoresEntityAndSavesChanges()
    {
        // Arrange
        var command = new RestoreDisputeTypeCommand(DisputeTypeId.New());

        // Create an entity and explicitly delete it so it requires restoring
        var disputeType = DisputeType.Create("Wrong Item", "Received completely different item").Value;
        disputeType.Delete(); // IsActive becomes false

        _disputeTypeRepository.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(disputeType);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify the domain state changed
        disputeType.IsActive.ShouldBeTrue();

        // Verify the infrastructure commit was called
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}