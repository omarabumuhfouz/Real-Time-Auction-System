using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Domain.Shared; 
using MazadZone.Domain.Shared.Errors; 

namespace MazadZone.Application.Common.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(AddressErrors.MaxCityLength);

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(SharedConstainst.MaxStreetLength);

        RuleFor(x => x.Building)
            .NotEmpty()
            .MaximumLength(SharedConstainst.MaxBuildingLength);

        RuleFor(x => x.Landmark)
            .NotEmpty()
            .MaximumLength(SharedConstainst.MaxLandmarkLength);
    }
}