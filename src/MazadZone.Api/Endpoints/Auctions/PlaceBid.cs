using MazadZone.Application.Features.Auctions.Commands.PlaceBid;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Bidders;
using MazadZone.Domain.Orders;
using MazadZone.Domain.ValueObjects;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public record PlaceBidRequest(BidderId BidderId,string methodId, decimal Amount)
{
    public PlaceBidCommand ToCommand(AuctionId auctionId) => new(auctionId, BidderId, methodId,Money.Create(Amount, Currency.Jod).Value);
}

public static class PlaceBid
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/bids", HandleAsync)
            .WithName("PlaceBid")
            .WithOpenApi()
            .WithSummary("Places a bid on an auction")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] AuctionId auctionId,
        [FromBody] PlaceBidRequest? request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        var result = await sender.Send(request.ToCommand(auctionId), ct);
        return result.Match(onValue: _ => Results.Ok(), onError: e => e.ToProblem());
    }
}
