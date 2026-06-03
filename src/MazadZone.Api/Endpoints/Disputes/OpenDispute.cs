using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Disputes.Commands.OpenDispute;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Disputes;

public record OpenDisputeRequest(OrderId OrderId,DisputeTypeId DisputeTypeId, string Title, string Description, List<ImageModelDto>? Images = null)
{
    public OpenDisputeCommand ToCommand() => new(OrderId, DisputeTypeId, Title, Description, Images);
}

public static class OpenDispute
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
           .RequireAuthorization(Policies.Shared)
           .WithOpenApi()
           .WithSummary("Open a dispute for an order")
           .WithDescription("Initiates a formal dispute for an order, which pauses any pending payouts and flags the transaction for administrative review. A valid reason must be provided. Returns a 409 Conflict if the order is already disputed, or if it is in a state that cannot be disputed (e.g., canceled). **Requires authentication (any role).**")
           .Accepts<OpenDisputeRequest>("application/json")
           .Produces<Guid>(StatusCodes.Status200OK)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] OpenDisputeRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request.ToCommand(), ct);

        return result.Match(
            onValue: value => Results.Ok(value),
            onError: errors => errors.ToProblem());
    }
}