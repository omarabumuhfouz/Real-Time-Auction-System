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
           .WithSummary("Add feedback to a delivered order")
           .WithDescription("Allows a buyer to submit a rating and comment for a completed/delivered order. Returns a 409 Conflict if the order is not yet delivered, or if feedback has already been submitted for this order.")
           .Accepts<AddFeedbackRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // e.g., Rating is outside the 1-5 range, or comment is too long
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // User is logged in, but is not the buyer of this order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule violations (e.g., Order not delivered yet, or feedback already exists)
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