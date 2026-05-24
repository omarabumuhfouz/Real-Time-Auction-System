using MazadZone.Application.Features.Auctions.Commands.CreateAuction;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Auctions.ValueObjects;
using MazadZone.Domain.Categories;
using MazadZone.Domain.Sellers;
using MazadZone.Domain.ValueObjects;
using MediatR;

namespace MazadZone.Api.Endpoints.Auctions;

public record CreateAuctionRequest(
    SellerId SellerId,
    AddressDto ShippingAddress,
    decimal StartBidAmount,
    decimal MinBidAmount,
    DateTime StartTime,
    DateTime EndTime,
    string Title,
    string Description,
    List<ImageModelDto> Images,
    CategoryId CatigoryId)
{
    public CreateAuctionCommand ToCommand() => new(
        SellerId,
        ShippingAddress.ToAddress(),
        StartBidAmount,
        MinBidAmount,
        StartTime,
        EndTime,
        Title,
        Description,
        Images,
        CatigoryId);
}

public static class CreateAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .WithName("CreateAuction")
            .WithOpenApi()
            .WithSummary("Creates a new auction")
            .Produces<AuctionId>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateAuctionRequest? request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        var result = await sender.Send(request.ToCommand(), ct);

        return result.Match(
            onValue: auctionId => Results.Created($"/api/v1/auctions/{auctionId}", new { AuctionId = auctionId }),
            onError: e => e.ToProblem());
    }
}
