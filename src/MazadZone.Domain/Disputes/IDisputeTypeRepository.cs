using MazadZone.Domain.Repositories;
using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Domain.Disputes;

public interface IDisputeTypeRepository : IGenericRepository<DisputeType, DisputeTypeId>, IScopedService
{

}


