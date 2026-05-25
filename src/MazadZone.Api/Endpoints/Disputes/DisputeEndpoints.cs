namespace MazadZone.Api.Endpoints.Disputes;

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class DisputeEndpoints
{
    public static void MapDisputeEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var disputeGroup = app.MapGroup("api/v{version:apiVersion}/disputes")
                              .WithApiVersionSet(versionSet)
                              .MapToApiVersion(1, 0)
                              .WithTags("Dispute Management");

        // Map sub-slice query endpoints
        GetById.MapEndpoint(disputeGroup);
        GetOpens.MapEndpoint(disputeGroup);
        GetUnderReview.MapEndpoint(disputeGroup);
        GetResolved.MapEndpoint(disputeGroup);

        OpenDispute.MapEndpoint(disputeGroup);
        ResolveDispute.MapEndpoint(disputeGroup);

    }
}