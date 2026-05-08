namespace MazadZone.Api.Endpoints.Sellers;

public static class SellerEndpoints
{
    public static void MapSellerEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var sellerGroup = app.MapGroup("api/v{version:apiVersion}/sellers")
                           .WithApiVersionSet(versionSet)
                           .MapToApiVersion(1, 0)
                           .WithTags("Sellers Management");

        // Command Slices
        BecomeSeller.MapEndpoint(sellerGroup);
        UpdateBankDetails.MapEndpoint(sellerGroup);
        Verify.MapEndpoint(sellerGroup);

        // Query Slices
        GetPrivateDetails.MapEndpoint(sellerGroup);
        GetPublicProfile.MapEndpoint(sellerGroup);
        GetUnverified.MapEndpoint(sellerGroup);
    }
}