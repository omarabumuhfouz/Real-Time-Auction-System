namespace MazadZone.Api.Endpoints.DisputeTypes;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Features.DisputeTypes.Commands.Delete;
using MazadZone.Api.Constants;

public static class Delete
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", HandleAsync)
        //    .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Soft delete a Dispute Type")
           .WithDescription("Marks an existing dispute type as deleted without removing it from the database entirely. Returns 204 No Content on success.")
           .Produces(StatusCodes.Status204NoContent)
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
        var result = await sender.Send(new DeleteDisputeTypeCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}