#region Using Directives

using MazadZone.Api.Contracts.Bidders;
using MazadZone.Application.Features.Bidders.Commands.Register;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;
using MazadZone.Domain.Auctions;
using MechanicShop.Api.Extensions;

#endregion

namespace MazadZone.Api.Endpoints;

public static class BidderEndpoints
{
    public static void MapBidderEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var bidderGroup = app.MapGroup("api/v{version:apiVersion}/bidders")
                       .WithApiVersionSet(versionSet)
                       .MapToApiVersion(1, 0)
                       .WithTags("Bidder Management");


        bidderGroup.MapPost("/register", RegisterBidderAsync)
     .WithSummary("Registers a new bidder")
     .WithDescription("Creates a bidder profile with personal details and address.")
     .Produces(StatusCodes.Status201Created)
     .Produces(StatusCodes.Status400BadRequest)
     .Produces(StatusCodes.Status409Conflict)
     .Produces(StatusCodes.Status500InternalServerError);

        bidderGroup.MapGet("/{id:guid}", GetBidderProfileAsync)
     .WithSummary("Retrieves a Bidder's Profile")
     .WithDescription("Gets detailed profile information for a specific bidder using their unique identifier.")
     .Produces<BidderProfileDto>(StatusCodes.Status200OK)
     .Produces(StatusCodes.Status400BadRequest)
     .Produces(StatusCodes.Status404NotFound)
     .Produces(StatusCodes.Status500InternalServerError);


    }


    private static async Task<IResult> RegisterBidderAsync(
     [FromBody] RegisterBidderRequest request,
     [FromServices] ISender sender,
     CancellationToken cancellationToken)
    {
        // Mapping Request to Command
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

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            onValue: bidderDto => Results.Created($"/bidders/{bidderDto.ProfileInfo.Id.Value}", bidderDto),
            onError: errors => errors.ToProblem());
    }
    private static async Task<IResult> GetBidderProfileAsync(
    BidderId id,
    [FromServices] ISender sender,
    CancellationToken cancellationToken)
    {
        // Mapping the Guid from the route to your strongly-typed BidderId
        var query = new GetBidderProfileQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            onValue: bidderDto => Results.Ok(bidderDto),
            onError: errors => errors.ToProblem());
    }
}