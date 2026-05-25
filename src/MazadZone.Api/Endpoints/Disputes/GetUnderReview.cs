namespace MazadZone.Api.Endpoints.Disputes;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MediatR;
using MazadZone.Application.Features.Disputes.Queries;
using MazadZone.Application.Features.Disputes.Queries.GetUnderReview;
using MazadZone.Api.Constants;

public static class GetUnderReview
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/under-review", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Get a list of all disputes currently under review")
           .WithDescription("Retrieves a read-only list of all disputes that are actively being investigated by the support team.")
           .Produces<IReadOnlyList<DisputeDto>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetUnderReviewDisputesQuery(), ct);

        return result.Match(
            onValue: disputes => Results.Ok(disputes),
            onError: errors => errors.ToProblem());
    }
}