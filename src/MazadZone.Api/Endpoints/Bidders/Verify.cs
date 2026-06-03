using MazadZone.Application.Features.Bidders.Commands.Verify;

namespace MazadZone.Api.Endpoints.Bidders;

public static class Verify
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/verify", HandleAsync)
            .RequireAuthorization(Policies.AdminOnly) 
            .WithSummary("Verify a bidder's identity")
            .WithDescription("Marks a specific bidder profile as verified. This operation updates the bidder's status and is typically restricted to administrative roles.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized) 
            .ProducesProblem(StatusCodes.Status403Forbidden) 
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new VerifyBidderCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}