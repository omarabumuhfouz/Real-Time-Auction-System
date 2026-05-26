using MazadZone.Application.Features.Users.Commands.AddPaymentMethod;
using MazadZone.Domain.Users.Enums;
using MazadZone.Api.Extensions;

namespace MazadZone.Api.Endpoints.Users;

/// <summary>Request body for adding a payment method (UserId is taken from the JWT, not from the body).</summary>
public sealed record AddPaymentMethodRequest(
    string Last4Digits,
    int ExpiryMonth,
    int ExpiryYear,
    string CardholderName,
    CardBrand Brand,
    string GatewayToken,
    bool IsDefault);

public static class AddPaymentMethod
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/me/payment-methods", HandleAsync)
           .RequireAuthorization()
           .WithSummary("Add a payment method")
           .WithDescription("Saves a new payment method for the authenticated user. The gateway token (e.g. Stripe pm_xxx) is stored; raw card numbers are never accepted. A user can hold at most 5 payment methods.")
           .Produces<AddPaymentMethodResponse>(StatusCodes.Status201Created)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status409Conflict)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] AddPaymentMethodRequest request,
        HttpContext httpContext,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        if (userId is null)
            return Results.Unauthorized();

        var command = new AddPaymentMethodCommand(
            userId.Value,
            request.Last4Digits,
            request.ExpiryMonth,
            request.ExpiryYear,
            request.CardholderName,
            request.Brand,
            request.GatewayToken,
            request.IsDefault);

        var result = await sender.Send(command, ct);

        return result.Match(
            response => Results.Created($"/api/v1/users/me/payment-methods/{response.Id}", response),
            e => e.ToProblem());
    }
}
