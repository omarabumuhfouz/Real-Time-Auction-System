using MazadZone.Application.Features.Sellers.Queries.GetPublicProfile;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Api.Endpoints.Sellers;

public static class GetPublicProfile
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/public", HandleAsync)
           .AllowAnonymous() 
           .WithSummary("Retrieve a seller's public profile")
           .WithDescription("Fetches publicly visible information for a seller, such as their display name, overall rating, and basic statistics. This data is safe to display to unauthenticated users. Returns a 404 if the seller does not exist.")
           .Produces<PublicSellerProfileResponse>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed GUID in route
           .ProducesProblem(StatusCodes.Status404NotFound) // Seller not found
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetPublicSellerProfileQuery(id), ct);
        return result.Match(
            response => Results.Ok(response),
            e => e.ToProblem());
    }
}