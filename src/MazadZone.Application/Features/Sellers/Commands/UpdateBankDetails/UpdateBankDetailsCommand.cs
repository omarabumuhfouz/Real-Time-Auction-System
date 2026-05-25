using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Application.Features.Sellers.Commands.UpdateBankDetails;

public sealed record UpdateBankDetailsCommand(UserId SellerId, string NewAccountNumber) : ICommand<Unit>;