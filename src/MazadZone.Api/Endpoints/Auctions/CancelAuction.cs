using MazadZone.Application.Features.Auctions.Commands.CancelAuction;
using MazadZone.Domain.Auctions;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public record CancelAuctionRequest(string Reason)
{
    public CancelAuctionCommand ToCommand(AuctionId auctionId) => new(auctionId, Reason);
}

public static class CancelAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/cancel", HandleAsync)
            .WithName("CancelAuction")
            .WithOpenApi()
            .WithSummary("Cancels an auction")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] AuctionId auctionId,
        [FromBody] CancelAuctionRequest? request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        var result = await sender.Send(request.ToCommand(auctionId), ct);
        return result.Match(onValue: _ => Results.NoContent(), onError: e => e.ToProblem());
    }
}
