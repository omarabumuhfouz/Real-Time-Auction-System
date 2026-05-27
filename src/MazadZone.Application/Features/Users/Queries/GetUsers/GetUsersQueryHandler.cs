using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Application.Services; 

namespace MazadZone.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserQueries _userQueries;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(IUserQueries userQueries, ILogger<GetUsersQueryHandler> logger)
    {
        _userQueries = userQueries;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await _userQueries.GetUsersAsync(request.FilterParams, ct);
        
        return Result.Success(users ?? new List<UserDto>());
    }
}