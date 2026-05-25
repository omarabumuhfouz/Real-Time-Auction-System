namespace MazadZone.Api.Endpoints.DisputeTypes;

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public static class DisputeTypeEndpoints
{
    public static void MapDisputeTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var disputeTypeGroup = app.MapGroup("api/v{version:apiVersion}/dispute-types")
                                  .WithApiVersionSet(versionSet)
                                  .MapToApiVersion(1, 0)
                                  .WithTags("Dispute Types Management");

        // Map Commands
        Create.MapEndpoint(disputeTypeGroup);
        Update.MapEndpoint(disputeTypeGroup);
        Delete.MapEndpoint(disputeTypeGroup);
        Restore.MapEndpoint(disputeTypeGroup);

        // Map Queries
        GetById.MapEndpoint(disputeTypeGroup);
        GetAll.MapEndpoint(disputeTypeGroup);
    }
}