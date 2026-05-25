namespace MazadZone.Api.Endpoints.Disputes;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MediatR;
using MazadZone.Application.Features.Disputes.Queries;
using MazadZone.Application.Features.Disputes.Queries.GetOpens;
using MazadZone.Api.Constants;

public static class GetOpens
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/open", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Get a list of all open disputes")
           .WithDescription("Retrieves a read-only list of all currently open and unassigned disputes.")
           .Produces<IReadOnlyList<DisputeDto>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetOpenDisputesQuery(), ct);

        return result.Match(
            onValue: disputes => Results.Ok(disputes),
            onError: errors => errors.ToProblem());
    }
}