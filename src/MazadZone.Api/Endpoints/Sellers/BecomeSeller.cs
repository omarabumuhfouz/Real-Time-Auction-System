using MazadZone.Application.Features.Sellers.Commands.BecomeSeller;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Api.Endpoints.Sellers;

public record BecomeSellerRequest(string BankAccountNumber);

public static class BecomeSeller
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/become-seller", HandleAsync)
        //    .RequireAuthorization("BidderPolicy") 
           .WithSummary("Promote a user to a seller")
           .WithDescription("Upgrades an existing bidder/user account to a seller profile by linking their bank account details. Returns a 409 Conflict if the user is already a registered seller.")
           .Accepts<BecomeSellerRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If restricted by policies
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        [FromBody] BecomeSellerRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new BecomeSellerCommand(id, request.BankAccountNumber), ct);

        return result.Match(
            _ => Results.NoContent(),
             e => e.ToProblem());
    }
}