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
        app.MapPost("/register", RegisterBidderAsync)
           .WithSummary("Registers a new bidder")
           .WithDescription("Creates a bidder profile with personal details and address.")
           .Produces(StatusCodes.Status201Created)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> RegisterBidderAsync(
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
            onValue: bidderDto => Results.Created($"/api/v1/bidders/{bidderDto.ProfileInfo.Id.Value}", bidderDto),
            onError: errors => errors.ToProblem());
    }
}