using MazadZone.Application.Features.Sellers.Queries.GetUnverifiedSellers;

namespace MazadZone.Api.Endpoints.Sellers;

public static class GetUnverified
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/unverified", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Retrieve all unverified sellers")
           .WithDescription("Fetches a list of all seller profiles that are currently pending verification. This endpoint is typically consumed by administrative back-office dashboards to review and approve new sellers.")
           .Produces<IReadOnlyList<UnverifiedSellerSummaryResponse>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If role-based policies are used
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetUnverifiedSellersQuery(), ct);
        return result.Match(
            sellers => Results.Ok(sellers),
            e => e.ToProblem());
    }
}