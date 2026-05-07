using MazadZone.Application.Features.Authentication.DTOs;
using MazadZone.Application.Features.Bidders.DTOs;

namespace MazadZone.Application.Features.Users.DTOs;

public record RegisterBidderDto(BidderProfileDto ProfileInfo, TokenDto TokenInfo);
