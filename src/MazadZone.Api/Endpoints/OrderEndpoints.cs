#region Using Directives
using MazadZone.Api.Contracts.Orders;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Application.Features.Orders.Commands.ShipOrder;
using MazadZone.Application.Features.Orders.Commands.ConfirmOrder;
using MazadZone.Application.Features.Orders.Commands.DeliverOrder;
using MazadZone.Application.Features.Orders.Commands.CancelOrder;
using MazadZone.Application.Features.Orders.Queries.GetOrderDetails;
using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.SearchOrders;
using MazadZone.Application.Features.Orders.Queries.GetOrderByWinningBid;
using MazadZone.Application.Features.Orders.Queries.GetSellerStats;
using MazadZone.Application.Features.Orders.Queries.GetGlobalStats;
using MazadZone.Domain.Orders;
using MechanicShop.Api.Extensions;
using MazadZone.Domain.Auctions;
using AutoMapper;
#endregion

namespace MazadZone.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var orderGroup = app.MapGroup("api/v{version:apiVersion}/orders")
                       .WithApiVersionSet(versionSet)
                       .MapToApiVersion(1, 0)
                       .WithTags("Order Queries");

        #region Order Commands

        orderGroup.MapPost("/", CreateOrderAsync)
            .WithSummary("Creates a new Order")
            .WithDescription("Initiates a post-auction order transaction by creating a new Order instance mapping bidder, bid, address, and transaction data.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);

        orderGroup.MapPost("/{id:guid}/confirm", ConfirmOrderAsync)
            .WithSummary("Confirms an Order")
            .WithDescription("Transitions the order status to Confirmed.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        orderGroup.MapPost("/{id:guid}/ship", ShipOrderAsync)
            .WithSummary("Ships an Order")
            .WithDescription("Transitions the order status to Shipped.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        orderGroup.MapPost("/{id:guid}/deliver", DeliverOrderAsync)
            .WithSummary("Delivers an Order")
            .WithDescription("Transitions the order status to Delivered.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        orderGroup.MapPost("/{id:guid}/cancel", CancelOrderAsync)
            .WithSummary("Cancels an Order")
            .WithDescription("Transitions the order status to Cancelled.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        orderGroup.MapPost("/{id:guid}/dispute", OpenDisputeAsync)
            .WithSummary("Opens a Dispute on an Order")
            .WithDescription("Initiates a dispute regarding an order that has been delivered or encounters issues.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        orderGroup.MapPost("/{id:guid}/dispute/resolve", ResolveDisputeAsync)
            .WithSummary("Resolves an open Dispute")
            .WithDescription("Provides resolution details to close an active dispute.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        orderGroup.MapPost("/{id:guid}/feedback", AddFeedbackAsync)
            .WithSummary("Adds Feedback to a Delivered Order")
            .WithDescription("Allows users to leave a rating and comment post-delivery.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        #endregion

        #region Order Queries

        orderGroup.MapGet("/{id:guid}", GetOrderDetailsAsync)
         .WithName("GetOrderDetails")
         .Produces<OrderDetailsDto>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status404NotFound);

        orderGroup.MapPost("/search", SearchOrdersAsync)
             .WithName("SearchOrders")
             .Produces<PagedList<OrderSummaryDto>>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status400BadRequest);

        orderGroup.MapGet("/by-bid/{bidId:guid}", GetOrderByWinningBidAsync)
             .WithName("GetOrderByWinningBid")
             .Produces<OrderDetailsDto>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound);

        orderGroup.MapGet("/stats/seller/{sellerId:guid}", GetSellerStatsAsync)
             .WithName("GetSellerStats")
             .Produces<SellerOrderStatsDto>(StatusCodes.Status200OK);

        orderGroup.MapGet("/stats/global", GetGlobalStatsAsync)
             .WithName("GetGlobalStats")
             .Produces<AdminGlobalStatsDto>(StatusCodes.Status200OK)
             .RequireAuthorization("AdminOnly");

             #endregion
    }

    #region  Order Commands Methods
    private static async Task<IResult> CreateOrderAsync(
        [FromBody] CreateOrderRequest? request,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken ct)
    {
        if (request is null)
            return Results.BadRequest("Request body cannot be null.");

        var result = await sender.Send(request.ToCommand(mapper), ct);

        return result.Match(
            onValue: orderId => Results.Created($"/api/orders/{orderId}", new { OrderId = orderId }),
            onError: e => e.ToProblem());
    }

    private static async Task<IResult> ConfirmOrderAsync(
        OrderId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ConfirmOrderCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> ShipOrderAsync(
        OrderId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ShipOrderCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> DeliverOrderAsync(
        OrderId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new DeliverOrderCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> CancelOrderAsync(
    OrderId id,
    [FromServices] ISender sender,
    CancellationToken ct)
    {
        var result = await sender.Send(new CancelOrderCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> OpenDisputeAsync(
        OrderId id,
        [FromBody] OpenDisputeRequest? request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null) return Results.BadRequest("Request body cannot be null.");

        var result = await sender.Send(request.ToCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> ResolveDisputeAsync(
        OrderId id,
        [FromBody] ResolveDisputeRequest? request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null) return Results.BadRequest("Request body cannot be null.");

        var result = await sender.Send(request.ToCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    private static async Task<IResult> AddFeedbackAsync(
        OrderId id,
        [FromBody] AddFeedbackRequest? request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null) return Results.BadRequest("Request body cannot be null.");

        var result = await sender.Send(request.ToCommand(id), ct);

        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }

    #endregion

    #region Order Queries Methods
    private static async Task<IResult> GetOrderDetailsAsync(
            OrderId id,
            ISender sender,
            CancellationToken ct)
    {
        var query = new GetOrderDetailsQuery(id);
        var result = await sender.Send(query, ct);

        return result.Match(
           onValue: value => Results.Ok(value),
           onError: e => e.ToProblem());
    }

    private static async Task<IResult> SearchOrdersAsync(
        [FromBody] OrderSearchFilter filter,
        ISender sender,
        CancellationToken ct)
    {
        var query = new SearchOrdersQuery(filter);
        var result = await sender.Send(query, ct);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetOrderByWinningBidAsync(
        BidId bidId,
        ISender sender,
        CancellationToken ct)
    {
        var query = new GetOrderByWinningBidQuery(bidId);
        var result = await sender.Send(query, ct);

        return result.Match(
              onValue: value => Results.Ok(value),
              onError: e => e.ToProblem()
        );
   }

    private static async Task<IResult> GetSellerStatsAsync(
        SellerId sellerId,
        ISender sender,
        CancellationToken ct)
    {
        var query = new GetSellerStatsQuery(sellerId);
        var result = await sender.Send(query, ct);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetGlobalStatsAsync(
        ISender sender,
        CancellationToken ct)
    {
        var query = new GetGlobalStatsQuery();
        var result = await sender.Send(query, ct);

        return Results.Ok(result);
    }
    #endregion
}