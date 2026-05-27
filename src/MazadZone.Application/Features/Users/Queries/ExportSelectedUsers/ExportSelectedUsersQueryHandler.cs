using System.Text;
using MazadZone.Application.Features.Users.Queries.ExportUsers; 
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Users.Queries.ExportSelectedUsers;

public class ExportSelectedUsersQueryHandler : IQueryHandler<ExportSelectedUsersQuery, ExportFileResponse>
{
    private readonly IUserQueries _userQueries;

    public ExportSelectedUsersQueryHandler(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportSelectedUsersQuery request, CancellationToken ct)
    {
        // 1. Fetch exact matching records from our dedicated Dapper query
        var users = await _userQueries.ExportSelectedUsersAsync(request.SelectedUserIds, ct);

        // 2. Build CSV string
        var csvBuilder = new StringBuilder();
        csvBuilder.AppendLine("Id,Full Name,Email,Phone Number,Role,Status,Joined At,Last Login");

        foreach (var user in users)
        {
            csvBuilder.AppendLine(
                $"{user.Id}," +
                $"\"{user.FullName}\"," +
                $"\"{user.Email}\"," +
                $"\"{user.PhoneNumber}\"," +
                $"{user.Role}," +
                $"{user.Status}," +
                $"{user.JoinedAt:yyyy-MM-dd HH:mm:ss}," +
                $"{user.LastLogin:yyyy-MM-dd HH:mm:ss}");
        }

        // 3. Convert to File payload
        var fileContents = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        var fileName = $"Selected_Users_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        
        return new ExportFileResponse(fileContents, "text/csv", fileName);
    }
}