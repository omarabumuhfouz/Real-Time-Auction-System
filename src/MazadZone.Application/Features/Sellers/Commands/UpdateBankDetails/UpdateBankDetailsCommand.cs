using MazadZone.Domain.Auctions;

namespace MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;

public sealed record UpdateBankDetailsCommand(SellerId SellerId, string NewAccountNumber) : ICommand<Unit>;