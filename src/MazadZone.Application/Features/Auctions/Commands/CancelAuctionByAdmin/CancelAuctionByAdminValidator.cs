using FluentValidation;

namespace MazadZone.Application.Features.Auctions.Commands.CancelAuctionByAdmin;

public sealed class CancelAuctionByAdminValidator : AbstractValidator<CancelAuctionByAdminCommand>
{
    public CancelAuctionByAdminValidator()
    {
        RuleFor(x => x.AuctionId)
            .NotEmpty();
    }
}
