using MazadZone.Application.Features.Users.Queries.ExportUsers; 

namespace MazadZone.Application.Features.Users.Queries.ExportSelectedUsers;

public record ExportSelectedUsersQuery(List<Guid> SelectedUserIds) : IQuery<ExportFileResponse>;