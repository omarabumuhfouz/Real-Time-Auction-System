using MazadZone.Application.Features.Auctions.Commands.ActivateAuction;
using MazadZone.Domain.Auctions;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public static class ActivateAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/activate", HandleAsync)
            .WithName("ActivateAuction")
            .WithOpenApi()
            .WithSummary("Activates an auction")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] AuctionId auctionId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ActivateAuctionCommand(auctionId), ct);
        return result.Match(onValue: _ => Results.NoContent(), onError: e => e.ToProblem());
    }
}
