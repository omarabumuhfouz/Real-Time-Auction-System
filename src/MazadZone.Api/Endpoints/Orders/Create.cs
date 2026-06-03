using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Application.Features.Orders.Commands.Create;

namespace MazadZone.Api.Endpoints.Orders;

// BidderId is intentionally excluded — it is bound from the authenticated user's JWT claims.
public record CreateOrderRequest(
    AuctionId AuctionId,
    BidId WinningBidId,
    AddressDto ReceiptAddress,
    decimal Amount)
{
    public CreateOrderCommand ToCommand(UserId bidderId) => new(
        AuctionId,
        bidderId,
        WinningBidId,
        ReceiptAddress,
        Amount);
}

public static class Create
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/", HandleAsync)
           .RequireAuthorization(Policies.BidderOnly)
           .WithOpenApi()
           .WithSummary("Create a new order")
           .WithDescription("Initiates a post-auction order transaction for the winning bidder. The bidder identity is taken from the JWT. Requires the auction ID, the winning bid ID, and the shipping address. Returns a 409 Conflict if an order has already been created for this auction or if the provided bid was not the actual winner.")
           .Accepts<CreateOrderRequest>("application/json")
           .Produces(StatusCodes.Status201Created)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateOrderRequest? request,
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null) return Results.BadRequest("Request body cannot be null.");

        var result = await sender.Send(request.ToCommand(boundUserId.Value), ct);

        return result.Match(
            onValue: orderId => Results.Created($"/api/orders/{orderId}", new { OrderId = orderId }),
            onError: e => e.ToProblem());
    }
}