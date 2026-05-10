namespace MazadZone.Application.Features.Orders.Commands.Create;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.BidderId).NotEmpty();
        RuleFor(x => x.WinningBidId).NotEmpty();
        RuleFor(x => x.ReceiptAddressId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.DepositCaptureTransactionId).NotEmpty();
    }
}