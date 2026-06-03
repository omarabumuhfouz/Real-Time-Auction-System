using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Auctions.Commands.PlaceBid;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Orders;
using MazadZone.Domain.ValueObjects;

namespace MazadZone.Api.Endpoints.Auctions;

// BidderId is intentionally excluded — it is bound from the authenticated user's JWT claims.
public record PlaceBidRequest(string methodId, decimal Amount)
{
    public PlaceBidCommand ToCommand(AuctionId auctionId, UserId bidderId) =>
        new(auctionId, bidderId, methodId, Money.Create(Amount, Currency.Jod).Value);
}

public static class PlaceBid
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/bids", HandleAsync)
            .WithName("PlaceBid")
            .WithOpenApi()
            .WithSummary("Places a bid on an auction")
            .WithDescription("Places a new bid on an active auction on behalf of the authenticated bidder. The bidder identity is taken from the JWT — do not pass a BidderId in the body. The bid amount must meet the auction's minimum bid increment. **Requires Bidder role.**")
            .RequireAuthorization(Policies.BidderOnly)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] AuctionId auctionId,
        [FromBody] PlaceBidRequest? request,
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        var result = await sender.Send(request.ToCommand(auctionId, boundUserId.Value), ct);
        return result.Match(onValue: _ => Results.Ok(), onError: e => e.ToProblem());
    }
}
