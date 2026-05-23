using MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;
using MazadZone.Domain.Sellers;

namespace MazadZone.Api.Endpoints.Sellers;

public record UpdateBankDetailsRequest(string NewAccountNumber);

public static class UpdateBankDetails
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}/bank-details", HandleAsync)
           // .RequireAuthorization()// for Seller 
           .WithSummary("Update seller bank account details")
           .WithDescription("Updates the linked bank account number for a specific seller. Because this is a sensitive financial operation, domain rules may revert a verified seller back to an 'unverified' status pending manual review.")
           .Accepts<UpdateBankDetailsRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized) // If RequireAuthorization is used
           .ProducesProblem(StatusCodes.Status403Forbidden) // If the logged-in user tries to change someone else's bank details
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute]SellerId id,
        [FromBody] UpdateBankDetailsRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new UpdateBankDetailsCommand(id, request.NewAccountNumber), ct);
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}