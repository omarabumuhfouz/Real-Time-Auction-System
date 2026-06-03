using MazadZone.Application.Features.Payments.Commands.PayRemainingAmount;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MazadZone.Api.Endpoints.Payments;

public record PayRemainingAmountRequest(string PaymentMethodId)
{
    public PayRemainingAmountCommand ToCommand(Guid orderId) => new(orderId, PaymentMethodId);
}

public static class PayRemaining
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{orderId}/pay-remaining", HandleAsync)
            .WithName("PayRemainingAmount")
            .WithOpenApi()
            .WithSummary("Pays the remaining amount for an order after authorization")
            .WithDescription("Charges the remaining balance on an order for a won auction using the specified payment method. This is called after the initial payment authorization is confirmed. **Requires Bidder role.**")
            .RequireAuthorization(Policies.BidderOnly)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid orderId,
        [FromBody] PayRemainingAmountRequest? request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        var result = await sender.Send(request.ToCommand(orderId), ct);
        return result.Match(onValue: _ => Results.Ok(), onError: e => e.ToProblem());
    }
}
