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
           .WithDescription("Allows a seller to publicly reply to feedback left by a buyer on a specific completed order. Returns a 404 if the order is not found, and a 409 Conflict if no feedback exists yet or if a reply has already been submitted.")
           .Accepts<ReplyToFeedbackRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // For a malformed GUID, or if the 'ReplyText' is empty/too long
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing or invalid token
           .ProducesProblem(StatusCodes.Status403Forbidden) // Token is valid, but the user is not the seller of THIS order
           .ProducesProblem(StatusCodes.Status404NotFound) // Order does not exist
           .ProducesProblem(StatusCodes.Status409Conflict) // Domain rule violations (e.g., no feedback to reply to, or already replied)
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