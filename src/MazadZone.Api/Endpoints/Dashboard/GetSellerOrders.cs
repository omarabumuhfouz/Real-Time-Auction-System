using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Api.Constants;
using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Orders.Queries.GetSellerOrders;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class GetSellerOrders
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", HandleAsync)
           .RequireAuthorization(Policies.SellerOnly)
           .WithSummary("Get paginated orders for the Seller Dashboard")
           .WithDescription("Retrieves a paginated list of orders belonging to the authenticated seller. Use the optional 'status' query parameter (e.g., 'Pending', 'Shipped', or 'All') to filter the table.")
           .Produces<PagedList<OrderSummaryDto>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        BoundUserId sellerId,
        [FromServices] ISender sender,
        CancellationToken ct,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)

    {
        var result = await sender.Send(new GetSellerOrdersQuery(sellerId.Value, status, page, pageSize), ct);

        return result.Match(
            data => Results.Ok(data),
            error => error.ToProblem());
    }
}