namespace MazadZone.Api.Endpoints.DisputeTypes;

using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MazadZone.Features.DisputeTypes.Commands.Create;
using MazadZone.Api.Constants;

public static class Create
{
    public record Request(string Name, string Description);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
        //    .RequireAuthorization(Policies.AdminOnly) 
           .WithSummary("Create a new Dispute Type")
           .WithDescription("Creates a new dispute type configuration for the system. Returns a 201 Created with the location of the new resource. Returns 400 Bad Request if the input data fails validation, or 409 Conflict if a domain rule is violated (e.g., duplicate name).")
           .Produces(StatusCodes.Status201Created)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For invalid input data
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Lacks permission to create
           .ProducesProblem(StatusCodes.Status500InternalServerError); // Server/Database errors
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] Request request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new CreateDisputeTypeCommand(request.Name, request.Description), ct);

        return result.Match(
            onValue: id => Results.Created($"/api/v1/dispute-types/{id}", id),
            onError: errors => errors.ToProblem());
    }
}