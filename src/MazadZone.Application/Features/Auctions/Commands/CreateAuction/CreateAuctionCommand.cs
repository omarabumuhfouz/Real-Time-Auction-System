using MazadZone.Application.Features.Auctions.DTOs;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Auctions.Enums;
using MazadZone.Domain.Categories;

namespace MazadZone.Application.Features.Auctions.Commands.CreateAuction;

public sealed record CreateAuctionCommand(
    UserId SellerId,
    ItemStatus Status,
    string Condition,
    Address ShippingAddress,
    decimal StartBidAmount,
    decimal MinBidAmount,
    DateTime StartTime,
    DateTime EndTime,
    string Title,
    string Description,
    List<ImageModelDto> Images,
    CategoryId CatigoryId
    ) : ICommand<AuctionId>;
