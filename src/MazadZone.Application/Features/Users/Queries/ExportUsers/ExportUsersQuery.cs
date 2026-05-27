using MazadZone.Application.Features.Users.DTOs;

namespace MazadZone.Application.Features.Users.Queries.ExportUsers;
public record ExportUsersQuery(UserFilterParams FilterParams) : IQuery<ExportFileResponse>;
