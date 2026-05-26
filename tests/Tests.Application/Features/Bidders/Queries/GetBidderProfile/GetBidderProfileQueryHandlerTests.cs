using MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Bidders;

namespace Tests.Application.Features.Bidders.Queries.GetBidderProfile;

public class GetBidderProfileQueryHandlerTests : BidderBaseTest<GetBidderProfileQueryHandler>
{
    [Fact]
    public async Task Handle_BidderProfileDoesNotExist_ReturnsNotFoundError()
    {
        // Arrange
        var query = new GetBidderProfileQuery(UserId.New());

        // Simulate the Dapper/Read query returning null
        _bidderQueries.GetBidderProfile(query.BidderId)
            .Returns((BidderProfileDto?)null);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.TopError.ShouldBe(BidderErrors.NotFound);
    }

    [Fact]
    public async Task Handle_BidderExists_ReturnsProfile()
    {
        // Arrange
        var bidderId = UserId.New();
        var query = new GetBidderProfileQuery(bidderId);

        // Create a fake DTO response that matches your BidderProfileDto shape
        var expectedProfile = BidderHelper.CreateValidBidderProfileDto() with { Id = bidderId.Value };

        _bidderQueries.GetBidderProfile(query.BidderId)
            .Returns(expectedProfile);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        
        // Shouldly will check that the returned object matches the one we set up
        result.Value.ShouldBe(expectedProfile);
    }
}