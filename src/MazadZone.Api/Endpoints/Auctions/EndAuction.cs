using MazadZone.Application.Features.Auctions.Commands.EndAuction;
using MazadZone.Domain.Auctions;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public static class EndAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/end", HandleAsync)
            .WithName("EndAuction")
            .WithOpenApi()
            .WithSummary("Ends an auction")
            .WithDescription("Manually ends an active auction before its scheduled end time. Triggers winner determination and order creation. **Requires Admin role.**")
            .RequireAuthorization(Policies.AdminOnly)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] AuctionId auctionId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new EndAuctionCommand(auctionId), ct);
        return result.Match(onValue: _ => Results.NoContent(), onError: e => e.ToProblem());
    }
}
