using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Commands.Verify;

public sealed record VerifySellerCommand(SellerId SellerId) : ICommand<Unit>;