using MazadZone.Application.Features.Orders.Queries.SearchOrders;
using MazadZone.Application.Features.Orders.Queries.DTOs;
using MazadZone.Application.Common.Paging;

namespace MazadZone.Api.Endpoints.Orders;

public static class Search
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/search", HandleAsync)
           // .RequireAuthorization() // Highly recommended: Searching across orders typically requires an authenticated user or admin
           .WithName("SearchOrders")
           .WithSummary("Search and filter orders")
           .WithDescription("Retrieves a paginated list of orders based on provided search criteria (e.g., status, date range, or buyer/seller IDs). The returned list contains lightweight summary objects rather than full order details.")
           .Produces<PagedList<OrderSummaryDto>>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // e.g., PageNumber < 1, or EndDate is before StartDate
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but user lacks permission (e.g., non-admin trying to view all orders)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] OrderSearchFilter filter,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new SearchOrdersQuery(filter), ct);
        return result.Match(
            onValue: value => Results.Ok(value),
            onError: e => e.ToProblem()
        );
    }
}