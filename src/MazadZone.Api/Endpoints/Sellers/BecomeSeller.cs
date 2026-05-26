using MazadZone.Application.Features.Authentication.DTOs;
using MazadZone.Application.Features.Sellers.Commands.BecomeSeller;

namespace MazadZone.Api.Endpoints.Sellers;


public static class BecomeSeller
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/become-seller", HandleAsync)
        //    .RequireAuthorization("BidderPolicy") 
           .WithSummary("Promote a user to a seller")
           .WithDescription("Upgrades an existing bidder/user account to a seller profile by linking their bank account details. Returns a 409 Conflict if the user is already a registered seller.")
           .Produces<TokenDto>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If restricted by policies
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new BecomeSellerCommand(id), ct);

        return result.Match(
            value => Results.Ok(value),
             e => e.ToProblem());
    }
}