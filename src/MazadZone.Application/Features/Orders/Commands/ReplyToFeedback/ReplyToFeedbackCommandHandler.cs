using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Orders.Commands.ReplyToFeedback;

public class ReplyToFeedbackCommandHandler : ICommandHandler<ReplyToFeedbackCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReplyToFeedbackCommandHandler> _logger;

    public ReplyToFeedbackCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReplyToFeedbackCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(ReplyToFeedbackCommand command, CancellationToken ct)
    {
        ReplyToFeedbackLogs.LogReplyAttempt(_logger, command.OrderId);

        // 1. Fetch domain aggregate root
        var order = await _orderRepository.GetWithFeedback(command.OrderId, ct);
        if (order is null)
        {
            GlobalLogs.LogOrderNotFound(_logger, command.OrderId);
            return OrderErrors.NotFound;
        }

        // 2. Execute business rule state machine mutator
        var result = order.ReplyToFeedback(command.ReplyText);
        if (result.IsFailure)
        {
            ReplyToFeedbackLogs.LogReplyFailure(_logger, command.OrderId, result.TopError.Message);
            return result.TopError;
        }

        // 3. Persist transaction changes & dispatch domain events
        await _unitOfWork.SaveChangesAsync(ct);
        
        ReplyToFeedbackLogs.LogReplySuccess(_logger, command.OrderId);
        return Unit.Value;
    }
}