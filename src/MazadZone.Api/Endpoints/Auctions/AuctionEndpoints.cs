namespace MazadZone.Api.Endpoints.Auctions;

public static class AuctionEndpoints
{
    public static void MapAuctionEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var auctionGroup = app.MapGroup("api/v{version:apiVersion}/auctions")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1, 0)
            .WithTags("Auctions");

        GetMyBids.MapEndpoint(auctionGroup);
    }
}
