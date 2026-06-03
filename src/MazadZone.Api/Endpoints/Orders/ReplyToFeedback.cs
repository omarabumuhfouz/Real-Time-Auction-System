using MazadZone.Domain.Orders;

namespace MazadZone.Application.Features.Orders.Commands.ReplyToFeedback;

public static class ReplyToFeedback
{
    public record ReplyToFeedbackRequest(string ReplyText);

    public static void MapEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders/{orderId:guid}/feedback/reply", HandleAsync)
           .RequireAuthorization(Policies.SellerOnly)
           .WithName("ReplyToOrderFeedback")
           .WithSummary("Reply to order feedback")
           .WithDescription("Allows the seller to publicly reply to feedback left by the buyer on a specific completed order. Returns a 404 if the order is not found, and a 409 Conflict if no feedback exists yet or if a reply has already been submitted. **Requires Seller role.**")
           .Accepts<ReplyToFeedbackRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict)
           .ProducesProblem(StatusCodes.Status500InternalServerError)
           .WithOpenApi();
    }

private static async Task<IResult> HandleAsync(
        [FromRoute] OrderId orderId,
        [FromBody] ReplyToFeedbackRequest request,
        [FromServices] ISender mediator,
        CancellationToken ct)
    {
        var result = await mediator.Send(new ReplyToFeedbackCommand(orderId, request.ReplyText), ct);

        return result.Match(
            onValue: value => Results.NoContent(),
            onError: errors => errors.ToProblem());
    }
}