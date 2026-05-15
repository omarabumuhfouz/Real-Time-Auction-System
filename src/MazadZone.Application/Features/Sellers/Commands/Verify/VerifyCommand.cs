using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Commands.Verify;

public sealed record VerifyCommand(SellerId SellerId) : ICommand<Unit>;