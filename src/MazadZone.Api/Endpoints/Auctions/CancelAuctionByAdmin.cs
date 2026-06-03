using MazadZone.Application.Features.Auctions.Commands.CancelAuctionByAdmin;
using MazadZone.Domain.Auctions;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public static class CancelAuctionByAdmin
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{auctionId}/cancel-by-admin", HandleAsync)
            .WithName("CancelAuctionByAdmin")
            .WithOpenApi()
            .WithSummary("Cancels an auction by admin")
            .WithDescription("Admin override to forcefully cancel any auction regardless of its current state or seller. Used for policy violations or system-level cancellations. **Requires Admin role.**")
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
        var result = await sender.Send(new CancelAuctionByAdminCommand(auctionId), ct);
        return result.Match(onValue: _ => Results.NoContent(), onError: e => e.ToProblem());
    }
}
