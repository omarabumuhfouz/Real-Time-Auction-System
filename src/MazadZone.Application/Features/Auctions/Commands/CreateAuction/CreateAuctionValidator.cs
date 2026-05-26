using FluentValidation;
using MazadZone.Application.Services;
using MazadZone.Domain.Auctions;
using MazadZone.Domain.Shared;

namespace MazadZone.Application.Features.Auctions.Commands.CreateAuction;

public sealed class CreateAuctionValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.Condition)
            .NotEmpty()
            .WithMessage("Condition description is required.")
            .MaximumLength(SharedConstainst.MaxDescriptionLength)
            .WithMessage($"Condition description must not exceed {SharedConstainst.MaxDescriptionLength} characters.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("A valid item condition status is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Item title is required.")
            .MaximumLength(AuctionConstants.MaxTitleLength)
            .WithMessage($"Item title must not exceed {AuctionConstants.MaxTitleLength} characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Item description is required.")
            .MaximumLength(SharedConstainst.MaxDescriptionLength)
            .WithMessage($"Item description must not exceed {SharedConstainst.MaxDescriptionLength} characters.");

        RuleFor(x => x.SellerId)
            .NotEmpty();

        RuleFor(x => x.ShippingAddress)
            .NotEmpty();

        RuleFor(x => x.StartBidAmount)    
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Start bid must be zero or a positive amount.");

        RuleFor(x => x.MinBidAmount)
            .GreaterThan(0m)
            .WithMessage("Minimum bid amount must be greater than zero.");


        RuleFor(x => x.StartTime)
            .GreaterThanOrEqualTo(dateTimeProvider.Now.AddSeconds(-30))
            .WithMessage("The auction start time must be in the future or current.")
            .LessThan(x => x.EndTime)
            .WithMessage("The auction start time must be before the end time.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("The auction end time must be after the start time.");
    }
}
