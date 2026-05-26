using MazadZone.Application.Features.Authentication.DTOs;

namespace MazadZone.Application.Features.Sellers.Commands.BecomeSeller;

public sealed record BecomeSellerCommand(UserId UserId) : ICommand<TokenDto>;