namespace MazadZone.Api.Endpoints.Disputes;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MediatR;
using MazadZone.Application.Features.Disputes.Queries;
using MazadZone.Application.Features.Disputes.Queries.GetById;
using MazadZone.Domain.Orders;
using MazadZone.Api.Constants;

public static class GetById
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Get a dispute by its unique identifier")
           .WithDescription("Retrieves the detailed information for a specific dispute using its unique identifier.")
           .Produces<DisputeDetailsDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] DisputeId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetDisputeByIdQuery(id), ct);

        return result.Match(
            onValue: dispute => Results.Ok(dispute),
            onError: errors => errors.ToProblem());
    }
}