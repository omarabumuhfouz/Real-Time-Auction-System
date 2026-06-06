using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Application.Features.Orders.Commands.Create;
using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Api.Constants;

namespace MazadZone.Api.Endpoints.Orders;

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
        app.MapPost("/", HandleAsync)
           .RequireAuthorization()
           .RequireAuthorization(Policies.BidderOnly)
           .WithSummary("Create a new order")
           .WithDescription("Initiates a post-auction order transaction for the winning bidder. Requires the auction ID, the winning bid ID, and the shipping address. Returns a 409 Conflict if an order has already been created for this auction or if the provided bid was not the actual winner.")
           .Accepts<CreateOrderRequest>("application/json")
           .Produces(StatusCodes.Status201Created)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For missing body or invalid address details
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // If policies restrict order creation
           .ProducesProblem(StatusCodes.Status404NotFound) // Auction, Bidder, or Bid does not exist
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