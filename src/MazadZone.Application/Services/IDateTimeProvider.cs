using MazadZone.Domain.Shared.Interfaces;

namespace MazadZone.Application.Services;
public interface IDateTimeProvider : ISingletonService
{
    DateTime Now { get; }
}