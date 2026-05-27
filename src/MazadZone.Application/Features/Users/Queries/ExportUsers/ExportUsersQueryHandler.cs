using System.Text;
using MazadZone.Application.Features.Users.Queries.GetUserGrowthTrends;
using MazadZone.Application.Services;

namespace MazadZone.Application.Features.Users.Queries.ExportUsers;

public class ExportUsersQueryHandler : IQueryHandler<ExportUsersQuery, ExportFileResponse>
{
    private readonly IUserQueries _userQueries;

    public ExportUsersQueryHandler(IUserQueries userQueries)
    {
        _userQueries = userQueries;
    }

    public async Task<Result<ExportFileResponse>> Handle(ExportUsersQuery request, CancellationToken ct)
    {
        // 1. Force the export flag to true to bypass SQL OFFSET/FETCH pagination
        var exportParams = request.FilterParams with { IsExport = true };

        // 2. Fetch all matching records from Dapper
        var pagedList = await _userQueries.GetUsersAsync(exportParams, ct);

        // 3. Build CSV string
        var csvBuilder = new StringBuilder();
        
        // Headers
        csvBuilder.AppendLine("Id,Full Name,Email,Phone Number,Role,Status,Joined At,Last Login");

        // Rows
        foreach (var user in pagedList.Items)
        {
            // Escape values with double quotes to prevent commas inside data from breaking columns
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

        // 4. Convert string to byte array
        var fileContents = Encoding.UTF8.GetBytes(csvBuilder.ToString());
        var fileName = $"Users_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
        
        return new ExportFileResponse(fileContents, "text/csv", fileName);
    }
}