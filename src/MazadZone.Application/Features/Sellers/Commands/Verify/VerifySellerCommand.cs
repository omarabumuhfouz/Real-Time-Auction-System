using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Sellers.Commands.Verify;

public sealed record VerifySellerCommand(UserId SellerId) : ICommand<Unit>;