namespace MazadZone.Api.Endpoints.DisputeTypes;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Features.DisputeTypes.Commands.Restore;
using MazadZone.Api.Constants;

public static class Restore
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/restore", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Restore a soft-deleted Dispute Type")
           .WithDescription("Removes the soft-delete marker from a previously deleted dispute type, making it active again.")
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
        var result = await sender.Send(new RestoreDisputeTypeCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}