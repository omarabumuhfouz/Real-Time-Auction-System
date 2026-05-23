using MazadZone.Application.Features.Sellers.Queries.GetPrivateDetails;
using MazadZone.Domain.Sellers;

namespace MazadZone.Api.Endpoints.Sellers;

public static class GetPrivateDetails
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/private", HandleAsync)
        //    .RequireAuthorization("SellerPolicy") 
           .WithSummary("Retrieve private details of a seller")
           .WithDescription("Fetches sensitive seller information, such as linked bank account details and financial metrics. Access should be strictly limited to the seller themselves or system administrators. Returns a 404 if the seller profile does not exist.")
           .Produces<PrivateSellerDetailsResponse>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // Malformed GUID
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but user is not authorized to view THIS seller's data
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]SellerId id,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetPrivateSellerDetailsQuery(id), ct);
        return result.Match(
            response => Results.Ok(response),
            e => e.ToProblem());
    }
}