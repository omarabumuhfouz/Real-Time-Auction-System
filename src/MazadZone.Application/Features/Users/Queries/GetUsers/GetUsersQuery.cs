using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Users.DTOs;

namespace MazadZone.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(UserFilterParams FilterParams) : IQuery<PagedList<UserDto>>;