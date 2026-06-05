using MazadZone.Application.Features.Users.Queries.GetPaymentMethods;

namespace MazadZone.Api.Endpoints.Users;

public static class GetPaymentMethods
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/me/payment-methods", HandleAsync)
           .RequireAuthorization()
           .WithSummary("Get saved payment methods")
           .WithDescription("Retrieves the list of payment methods saved by the currently authenticated user.")
           .Produces<IReadOnlyList<PaymentMethodResponse>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var userId = httpContext.User.GetUserId();
        if (userId is null)
            return Results.Unauthorized();

        var result = await sender.Send(new GetPaymentMethodsQuery(userId.Value), ct);

        return result.Match(
            response => Results.Ok(response),
            error => error.ToProblem());
    }
}
