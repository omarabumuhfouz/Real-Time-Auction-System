using MazadZone.Application.Features.Auctions.Commands.CreateAuction;
using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Auctions.Enums;
using MazadZone.Domain.Categories;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Api.Constants;

namespace MazadZone.Api.Endpoints.Auctions;

public record CreateAuctionRequest(
    string Status,
    string Condition,
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
    public CreateAuctionCommand ToCommand(UserId sellerId) => new(
        sellerId,
        Enum.TryParse<ItemStatus>(Status?.Replace(" ", ""), true, out var itemStatus) ? itemStatus : (ItemStatus)0,
        Condition,
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
            .RequireAuthorization(Policies.SellerOnly)
            .WithName("CreateAuction")
            .WithOpenApi()
            .WithSummary("Creates a new auction")
            .Produces<AuctionId>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateAuctionRequest? request,
        BoundUserId boundUserId,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        var result = await sender.Send(request.ToCommand(boundUserId.Value), ct);

        return result.Match(
            onValue: auctionId => Results.Created($"/api/v1/auctions/{auctionId}", new { AuctionId = auctionId }),
            onError: e => e.ToProblem());
    }
}

