using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Sellers.Commands.BecomeSeller;

public sealed record BecomeSellerCommand(BidderId BidderId, string BankAccountNumber) : ICommand<Unit>;