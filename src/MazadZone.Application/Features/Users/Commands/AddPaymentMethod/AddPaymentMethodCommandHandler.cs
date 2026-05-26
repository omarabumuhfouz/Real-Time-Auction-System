using MazadZone.Domain.Users.Entities;
using MazadZone.Domain.Repositories;
using MazadZone.Domain.Users.Errors;

namespace MazadZone.Application.Features.Users.Commands.AddPaymentMethod;

public sealed class AddPaymentMethodCommandHandler
    : ICommandHandler<AddPaymentMethodCommand, AddPaymentMethodResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddPaymentMethodCommandHandler> _logger;

    public AddPaymentMethodCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddPaymentMethodCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AddPaymentMethodResponse>> Handle(
        AddPaymentMethodCommand request,
        CancellationToken ct)
    {
        AddPaymentMethodLogs.LogAttempt(_logger, request.UserId);

        // 1. Verify the user exists (and load with their payment methods)
        var user = await _userRepository.GetByIdWithPaymentMethodsAsync(request.UserId, ct);
        if (user is null)
        {
            GlobalLogs.LogUserNotFound(_logger, request.UserId);
            return UserErrors.NotFound;
        }

        // 2. Delegate addition to user aggregate (domain validates invariants and toggles default status)
        var methodResult = user.AddPaymentMethod(
            request.Last4Digits,
            request.ExpiryMonth,
            request.ExpiryYear,
            request.CardholderName,
            request.Brand,
            request.GatewayToken,
            request.IsDefault);

        if (methodResult.IsFailure)
        {
            AddPaymentMethodLogs.LogDomainViolation(_logger, request.UserId, methodResult.TopError.Code);
            return methodResult.TopError;
        }

        var method = methodResult.Value;

        // 3. Mark user modified and persist
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(ct);

        AddPaymentMethodLogs.LogSuccess(_logger, request.UserId, method.Id);

        return new AddPaymentMethodResponse(
            method.Id,
            method.Last4Digits,
            method.ExpiryMonth,
            method.ExpiryYear,
            method.CardholderName,
            method.Brand,
            method.IsDefault,
            method.CreatedOnUtc);
    }
}
