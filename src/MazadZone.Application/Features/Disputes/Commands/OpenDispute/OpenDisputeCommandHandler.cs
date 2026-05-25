using MazadZone.Domain.Disputes;
using MazadZone.Domain.Repositories;

namespace MazadZone.Application.Features.Disputes.Commands.OpenDispute;

public class OpenDisputeCommandHandler : ICommandHandler<OpenDisputeCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDisputeRepository _disputeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OpenDisputeCommandHandler> _logger;

    public OpenDisputeCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<OpenDisputeCommandHandler> logger,
        IDisputeRepository disputeRepository
        )
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _disputeRepository = disputeRepository;
    }

    public async Task<Result<Unit>> Handle(OpenDisputeCommand request, CancellationToken ct)
    {
        // OpenDisputeLogs.LogAttempt(_logger, request.OrderId, request.Reason);

        var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);

        if (order is null)
        {
            GlobalLogs.LogOrderNotFound(_logger, request.OrderId);
            return OrderErrors.NotFound;
        }

        if (!order.IsDisputable) return OrderErrors.CannotDispute;

        var titleResult = Title.Create(request.Title);
        if (titleResult.IsFailure) return titleResult.TopError;

        var descriptionResult = Description.Create(request.Description);
        if (descriptionResult.IsFailure) return descriptionResult.TopError;

        var images = new List<Image>(); 

if (request.Images is not null && request.Images.Any())
{
    foreach(var imageDto in request.Images)
    {
        var image = Image.Create(imageDto.Path, imageDto.AltText, imageDto.isMain);
        if(image.IsFailure) return image.TopError;
        
        images.Add(image.Value); // ✅ Now this works perfectly
    }
}


        var dispute = Dispute.Open(request.OrderId, request.DisputeTypeId, titleResult.Value, descriptionResult.Value, images);


        _disputeRepository.Add(dispute);
        await _unitOfWork.SaveChangesAsync(ct);

        OpenDisputeLogs.LogSuccess(_logger, request.OrderId);

        return Unit.Value;
    }

}