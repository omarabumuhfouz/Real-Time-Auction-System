namespace Tests.Application.Features.DisputeTypes.Queries.GetById;

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Features.DisputeTypes.Queries.GetById;
using MazadZone.Features.DisputeTypes.Queries.GetAll; 
using MazadZone.Domain.Disputes; 

public class GetDisputeTypeByIdQueryHandlerTests : DisputeTypeBaseTest<GetDisputeTypeByIdQueryHandler>
{
    [Fact]
    public async Task Handle_DisputeTypeExists_ReturnsDisputeTypeDto()
    {
        // Arrange
        var command = new GetDisputeTypeByIdQuery(DisputeTypeId.New());

        var expectedDto = new DisputeTypeDto(
            command.DisputeTypeId.Value, 
            "Item Not Received", 
            "Buyer didn't get it", 
            true);

        // Mock the read-only queries repository to return the DTO
        _disputeTypeQueries.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns(expectedDto);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        
        // Verify mapping
        result.Value.Id.ShouldBe(command.DisputeTypeId.Value);
        result.Value.Name.ShouldBe(expectedDto.Name);
        result.Value.Description.ShouldBe(expectedDto.Description);
        result.Value.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_DisputeTypeDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var command = new GetDisputeTypeByIdQuery(DisputeTypeId.New());

        // Mock the queries repository to return null
        _disputeTypeQueries.GetByIdAsync(command.DisputeTypeId, Arg.Any<CancellationToken>())
            .Returns((DisputeTypeDto?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(DisputeTypeErrors.NotFound); // Assumes DisputeTypeErrors.NotFound exists
    }
}