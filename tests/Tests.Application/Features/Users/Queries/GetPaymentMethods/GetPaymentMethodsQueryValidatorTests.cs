namespace Tests.Application.Features.Users.Queries.GetPaymentMethods;

using MazadZone.Application.Features.Users.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryValidatorTests
{
    private readonly GetPaymentMethodsQueryValidator _validator;

    public GetPaymentMethodsQueryValidatorTests()
    {
        _validator = new GetPaymentMethodsQueryValidator();
    }

    [Fact]
    public void Validate_WhenUserIdIsValid_ShouldNotHaveAnyErrors()
    {
        var query = new GetPaymentMethodsQuery(UserId.New());
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveValidationError()
    {
        var query = new GetPaymentMethodsQuery(UserId.Empty);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
