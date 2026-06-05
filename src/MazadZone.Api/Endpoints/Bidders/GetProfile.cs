using MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Api.Infrastructure.Binding;

namespace MazadZone.Api.Endpoints.Bidders;

public static class GetProfile
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile", HandleAsync)
        .RequireAuthorization(Policies.BidderOnly) 
        .WithSummary("Retrieve a bidder's profile")
        .WithDescription("Fetches detailed profile information for a specific bidder using their unique identifier. Returns a 404 if the bidder does not exist.")
        .Produces<BidderProfileDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized) 
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetBidderProfileQuery(boundUserId.Value), ct);

        return result.Match(
            onValue: bidderDto => Results.Ok(bidderDto),
            onError: errors => errors.ToProblem());
    }
}