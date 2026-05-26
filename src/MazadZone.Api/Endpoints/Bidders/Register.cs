using MazadZone.Application.Features.Bidders.Commands.Register;
using MazadZone.Application.Features.Bidders.DTOs;

namespace MazadZone.Api.Endpoints.Bidders;
public record RegisterBidderRequest(
    string Email,
    string Password,
    string PhoneNumber,
    string NationalId,
    string FirstName,
    string SecondName,
    string ThirdName,
    string LastName,
    AddressDto Address
);
public static class Register
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", HandleAsync)
            .AllowAnonymous()
            .WithSummary("Register a new bidder")
            .WithDescription("Creates a new bidder account using personal details, national ID, and address. Returns the created bidder profile and a location header pointing to the new resource.")
            .Accepts<RegisterBidderRequest>("application/json")
            .Produces<RegisterBidderDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] RegisterBidderRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var command = new RegisterBidderCommand(
            request.Email,
            request.Password,
            request.PhoneNumber,
            request.NationalId,
            request.FirstName,
            request.SecondName,
            request.ThirdName,
            request.LastName,
            request.Address);

        var result = await sender.Send(command, ct);

        return result.Match(
            onValue: bidderDto => Results.Ok(bidderDto),
            onError: errors => errors.ToProblem());
    }
}