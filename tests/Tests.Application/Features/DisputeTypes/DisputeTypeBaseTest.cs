using Microsoft.Extensions.Logging;
using MazadZone.Domain.Disputes;

namespace Tests.Application.Features.DisputeTypes;

public abstract class DisputeTypeBaseTest<THandler> : GlobalTestBase
         where THandler : class
{
    protected readonly IDisputeTypeRepository _disputeTypeRepository;
    protected readonly IDisputeTypeQueries _disputeTypeQueries;

    protected readonly ILogger<THandler> Logger;

    protected THandler Handler => AutoMocker.CreateInstance<THandler>();

    protected DisputeTypeBaseTest()
    {
        _disputeTypeRepository = AutoMocker.GetSubstituteFor<IDisputeTypeRepository>();
        _disputeTypeQueries = AutoMocker.GetSubstituteFor<IDisputeTypeQueries>();
        Logger = AutoMocker.GetSubstituteFor<ILogger<THandler>>();
    }
}