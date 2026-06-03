using MazadZone.Application.Features.Sellers.Commands.Verify;

namespace MazadZone.Api.Endpoints.Sellers;

public static class Verify
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/verify", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Verify a seller's account")
           .WithDescription("Approves a pending seller profile, marking them as verified and granting them permission to create auctions or receive payouts. Returns a 409 Conflict if the seller is already verified.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed GUID
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If restricted by policies
           .ProducesProblem(StatusCodes.Status404NotFound) // Seller profile doesn't exist
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]UserId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new VerifySellerCommand(id), ct);
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}