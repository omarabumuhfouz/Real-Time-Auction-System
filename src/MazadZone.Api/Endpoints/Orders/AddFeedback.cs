using MazadZone.Application.Features.Orders.Commands.AddFeedback;
using MazadZone.Domain.Orders;

namespace MazadZone.Api.Endpoints.Orders;

public record AddFeedbackRequest(int Rating, string Comment)
{
    public AddFeedbackCommand ToCommand(OrderId orderId) => new(orderId, Rating, Comment);
}

public static class AddFeedback
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/feedback", HandleAsync)
           .RequireAuthorization(Policies.BidderOnly)
           .WithOpenApi()
           .WithSummary("Add feedback to a delivered order")
           .WithDescription("Allows the buyer to submit a rating and comment for a completed/delivered order. Returns a 409 Conflict if the order is not yet delivered, or if feedback has already been submitted. **Requires Bidder role.**")
           .Accepts<AddFeedbackRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] OrderId id,
        [FromBody] AddFeedbackRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(request.ToCommand(id), ct);
        return result.Match(
            onValue: _ => Results.NoContent(),
            onError: e => e.ToProblem());
    }
}