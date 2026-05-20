using MazadZone.Application.Features.Bidders.Queries.GetBidderProfile;
using MazadZone.Domain.Bidders;

namespace Tests.Application.Features.Bidders.Queries.GetBidderProfile;

public class GetBidderProfileValidatorTests
{
    private readonly GetBidderProfileQueryValidator _validator;

    public GetBidderProfileValidatorTests()
    {
        // Assumes your validator is named GetBidderProfileValidator
        _validator = new GetBidderProfileQueryValidator(); 
    }

    [Fact]
    public void Validate_ValidQuery_PassesValidation()
    {
        // Arrange
        var query = new GetBidderProfileQuery(BidderId.New());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_BidderIdIsEmpty_FailsValidation()
    {
        // Arrange
        var query = new GetBidderProfileQuery(BidderId.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        
        // This relies on your custom .MustBeValidBidderId() extension catching empty Guids
        result.ShouldHaveValidationErrorFor(x => x.BidderId);
    }
}