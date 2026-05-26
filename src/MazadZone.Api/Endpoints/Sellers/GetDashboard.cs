using MazadZone.Application.Features.Sellers.Queries.GetDashboard;
using MazadZone.Domain.Sellers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MazadZone.Api.Endpoints.Sellers;

public static class GetDashboard
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/dashboard", HandleAsync)
           .WithName("GetSellerDashboard")
           // TODO(security): .RequireAuthorization("SellerPolicy")
           .WithSummary("Retrieve dashboard statistics and auctions for a seller")
           .WithDescription("Fetches statistics including active, pending, sold, and unsold auctions count, along with a paginated list of auctions belonging to the specified seller.")
           .Produces<SellerDashboardResponse>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        [AsParameters] SellerDashboardFilter filter,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetSellerDashboardQuery(id, filter), ct);
        return result.Match(
            response => Results.Ok(response),
            e => e.ToProblem());
    }
}
