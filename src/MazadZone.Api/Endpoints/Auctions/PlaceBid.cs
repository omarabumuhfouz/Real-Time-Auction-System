using MazadZone.Application.Features.Auctions.Commands.PlaceBid;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Orders;
using MazadZone.Domain.ValueObjects;
using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Api.Constants;

namespace MazadZone.Api.Endpoints.Auctions;

public record PlaceBidRequest(string methodId, decimal Amount)
{
    public PlaceBidCommand ToCommand(AuctionId auctionId, UserId bidderId) => new(auctionId, bidderId, methodId, Money.Create(Amount, Currency.Jod).Value);
}

public static class PlaceBid
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/bids", HandleAsync)
            .RequireAuthorization(Policies.BidderOnly)
            .WithName("PlaceBid")
            .WithOpenApi()
            .WithSummary("Places a bid on an auction")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
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

