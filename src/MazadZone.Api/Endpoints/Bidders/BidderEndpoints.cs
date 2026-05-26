namespace MazadZone.Api.Endpoints.Bidders;

public static class BidderEndpoints
{
    public static void MapBidderEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var bidderGroup = app.MapGroup("api/v{version:apiVersion}/bidders")
                           .WithApiVersionSet(versionSet)
                           .MapToApiVersion(1, 0)
                           .WithTags("Bidder Management");

        // Register Slices
        Register.MapEndpoint(bidderGroup);
        GetProfile.MapEndpoint(bidderGroup);
        Verify.MapEndpoint(bidderGroup);
        GetMyBids.MapEndpoint(bidderGroup);
    }
}