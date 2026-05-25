namespace MazadZone.Api.Endpoints.DisputeTypes;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Features.DisputeTypes.Queries.GetById;

public static class GetById
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", HandleAsync)
           .RequireAuthorization()
           .WithSummary("Get a Dispute Type by ID")
           .WithDescription("Retrieves the detailed configuration of a specific dispute type by its unique identifier.")
           .Produces<DisputeTypeResponse>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For malformed GUID
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound) // Dispute Type does not exist
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] DisputeTypeId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetDisputeTypeByIdQuery(id), ct);

        return result.Match(
            onValue: value => Results.Ok(value),
            onError: errors => errors.ToProblem());
    }
}