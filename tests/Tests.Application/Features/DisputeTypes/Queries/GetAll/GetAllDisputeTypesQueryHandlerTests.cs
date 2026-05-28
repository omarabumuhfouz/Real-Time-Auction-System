namespace Tests.Application.Features.DisputeTypes.Queries.GetAll;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Features.DisputeTypes.Queries.GetAll;

public class GetAllDisputeTypesQueryHandlerTests : DisputeTypeBaseTest<GetAllDisputeTypesQueryHandler>
{
    [Fact]
    public async Task Handle_DisputeTypesExist_ReturnsPopulatedList()
    {
        // Arrange
        var command = new GetAllDisputeTypesQuery();

        var mockDisputeTypes = new List<DisputeTypeDto>
        {
            new DisputeTypeDto(Guid.NewGuid(), "Item Not Received",  "Buyer didn't get it" ,true),
            new DisputeTypeDto (Guid.NewGuid(), "Damaged Item",  "Arrived broken",true )
        }.AsReadOnly();

        // Mock the queries repository to return our fake data
        _disputeTypeQueries.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(mockDisputeTypes);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Handle_NoDisputeTypesExist_ReturnsEmptyList()
    {
        // Arrange
        var command = new GetAllDisputeTypesQuery();

        // 💡 TRICK: We force the repository to return null to test your 
        // fallback logic (`?? new List<DisputeTypeDto>()`)
        _disputeTypeQueries.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<DisputeTypeDto>?)null);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        // Verify it gracefully handled the null and returned an empty list instead
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }
}