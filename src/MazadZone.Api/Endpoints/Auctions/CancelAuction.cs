using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Auctions.Commands.CancelAuction;
using MazadZone.Domain.Auctions;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public record CancelAuctionRequest(string Reason)
{
    // TODO(security): The application-layer CancelAuctionCommandHandler MUST verify that
    // sellerId matches the auction's SellerId before cancelling, to prevent cross-seller cancellation.
    public CancelAuctionCommand ToCommand(AuctionId auctionId, UserId sellerId) => new(auctionId, Reason, sellerId);
}

public static class CancelAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/cancel", HandleAsync)
            .WithName("CancelAuction")
            .WithOpenApi()
            .WithSummary("Cancels an auction")
            .WithDescription("Cancels an auction owned by the authenticated seller. The seller identity is taken from the JWT and verified against the auction. A cancellation reason must be provided. **Requires Seller role.**")
            .RequireAuthorization(Policies.SellerOnly)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] AuctionId auctionId,
        [FromBody] CancelAuctionRequest? request,
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        var result = await sender.Send(request.ToCommand(auctionId, boundUserId.Value), ct);
        return result.Match(onValue: _ => Results.NoContent(), onError: e => e.ToProblem());
    }
}
