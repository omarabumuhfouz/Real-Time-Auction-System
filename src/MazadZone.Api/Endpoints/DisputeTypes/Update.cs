namespace MazadZone.Api.Endpoints.DisputeTypes;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Features.DisputeTypes.Commands.Update;
using MazadZone.Api.Constants;

public static class Update
{
    public record Request(string Name, string Description);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", HandleAsync)
        //    .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Update an existing Dispute Type")
           .WithDescription("Updates the name and description of an existing dispute type. Returns 204 No Content on success. Returns 404 Not Found if the ID does not exist.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For malformed GUID or invalid body data
           .ProducesProblem(StatusCodes.Status401Unauthorized) 
           .ProducesProblem(StatusCodes.Status403Forbidden) 
           .ProducesProblem(StatusCodes.Status404NotFound) // Dispute Type does not exist
           .ProducesProblem(StatusCodes.Status500InternalServerError); 
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] DisputeTypeId id,
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new UpdateDisputeTypeCommand(id, request.Name, request.Description), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}