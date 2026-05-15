using MazadZone.Domain.Auctions;
using MazadZone.Domain.Sellers;

namespace MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;

public sealed record UpdateBankDetailsCommand(SellerId SellerId, string NewAccountNumber) : ICommand<Unit>;