using System.Text;
using Dapper;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Common.Paging;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;
using MazadZone.Application.Services;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users.ValueObjects;
using Polly;

namespace MazadZone.Infrastructure.Repositories.Queries;

public class UserQueries : ResilientRepository, IUserQueries
{
    public UserQueries(ISqlConnectionFactory sqlFactory, IAsyncPolicy resiliencePolicy) : base(sqlFactory, resiliencePolicy) { }

    public async Task<Result<Address>> GetAddressByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT 
                City,
                Street,
                Building,
                Landmark
            FROM Bidders
            WHERE Id = @UserId;
        ";

        var addressDto = await ExecuteResilientAsync(connection =>
            connection.QueryFirstOrDefaultAsync<AddressDto>(sql, new { UserId = userId }));

        if (addressDto is null)
        {
            return Result.Failure<Address>(Error.NotFound("Address.NotFound", "Address not found for the user."));
        }

        return Address.Create(addressDto.City, addressDto.Street, addressDto.Building, addressDto.Landmark);
    }

    public async Task<Result<Email>> GetEmailByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var sql = @"
            SELECT Email
            FROM Users
            WHERE Id = @userId
        ";

        var email = await ExecuteResilientAsync(connection =>
            connection.QueryFirstOrDefaultAsync<string>(sql, new { userId = userId }));

        return Email.Create(email);
    }

    public async Task<ProfileSettingsResponse> GetProfileSettings(UserId userId, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT 
    u.Id,
    u.FirstName + ' ' + u.LastName AS FullName,
    u.Email,
    u.PhoneNumber,
    b.NationalId,
    b.City,
    b.Street,
    b.Building,
    b.Landmark
FROM Users u
JOIN Bidders b ON b.Id = u.Id
WHERE u.Id = @UserId;
            ";

        return await ExecuteResilientAsync(connection =>
               connection.QueryFirstOrDefaultAsync<ProfileSettingsResponse>(sql, new { UserId = userId.Value })
        );
    }

    public async Task<PagedList<UserDto>> GetUsersAsync(UserFilterParams filter, CancellationToken ct)
    {
        var parameters = new DynamicParameters();

        // 1. Build the shared WHERE clause filters
        var filterConditions = new StringBuilder(" WHERE 1 = 1 ");

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filterConditions.Append(@" 
        AND (FirstName LIKE @Search 
             OR LastName LIKE @Search 
             OR Email LIKE @Search 
             OR PhoneNumber LIKE @Search)");
            parameters.Add("Search", $"%{filter.SearchTerm}%");
        }

        // 💡 Change filter.JoinedDate to filter.ValidJoinedDate
        if (filter.ValidJoinedDate.HasValue)
        {
            // 💡 Access the DateTime value from ValidJoinedDate instead
            var startDate = filter.ValidJoinedDate.Value.Date;
            var endDate = startDate.AddDays(1);

            filterConditions.Append(" AND CreatedOnUtc >= @StartDate AND CreatedOnUtc < @EndDate");
            parameters.Add("StartDate", startDate);
            parameters.Add("EndDate", endDate);
        }

        // 2. Build Count Query String
        var countSql = $"SELECT COUNT(1) FROM Users {filterConditions}";

        // 3. Build Data Query String
        var dataSqlBuilder = new StringBuilder($@"
    SELECT 
        Id,
        (FirstName + ' ' + LastName) AS FullName,
        Email,
        PhoneNumber,
        CASE 
            WHEN (Roles & 4) = 4 THEN 'Admin'
            WHEN (Roles & 2) = 2 THEN 'Seller'
            WHEN (Roles & 1) = 1 THEN 'Bidder'
            ELSE 'None'
        END AS Role,
        CASE Status
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Suspended'
            WHEN 3 THEN 'Banned'
            ELSE 'Unknown'
        END AS Status,
        CreatedOnUtc AS JoinedAt,
        LastLogin
    FROM Users
    {filterConditions}");

        // Apply Sorting Rules
        var sortColumn = filter.SortBy?.ToLower() switch
        {
            "fullname" => "(FirstName + ' ' + LastName)",
            "joineddate" => "CreatedOnUtc",
            "lastlogin" => "LastLogin",
            "role" => "Roles",
            "status" => "Status",
            _ => "CreatedOnUtc"
        };
        var sortDirection = filter.IsAsc ? "ASC" : "DESC";
        dataSqlBuilder.Append($" ORDER BY {sortColumn} {sortDirection}");

        // Apply Pagination Limits if not exporting
        if (!filter.IsExport)
        {
            dataSqlBuilder.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
            parameters.Add("Offset", (filter.PageNumber - 1) * filter.PageSize);
            parameters.Add("PageSize", filter.PageSize);
        }

        // 4. Combine commands or stream multi-results
        return await ExecuteResilientAsync(async connection =>
        {
            if (!filter.IsExport)
            {
                var combinedSql = $"{countSql}; {dataSqlBuilder}";

                var command = new CommandDefinition(combinedSql, parameters, cancellationToken: ct);
                using var multi = await connection.QueryMultipleAsync(command);

                int totalCount = await multi.ReadFirstAsync<int>();
                var items = (await multi.ReadAsync<UserDto>()).ToList();

                // 💡 Instantiates your exact class constructor logic flawlessly
                return new PagedList<UserDto>(items, filter.PageNumber, filter.PageSize, totalCount);
            }
            else
            {
                // If exporting, skip pagination metrics entirely to maximize performance
                var command = new CommandDefinition(dataSqlBuilder.ToString(), parameters, cancellationToken: ct);
                var items = (await connection.QueryAsync<UserDto>(command)).ToList();

                return new PagedList<UserDto>(items, 1, items.Count, items.Count);
            }
        });
    }

public async Task<IReadOnlyList<UserDto>> ExportSelectedUsersAsync(IEnumerable<Guid> userIds, CancellationToken ct)
{
    var sql = @"
    SELECT 
        Id,
        (FirstName + ' ' + LastName) AS FullName,
        Email,
        PhoneNumber,
        CASE 
            WHEN (Roles & 4) = 4 THEN 'Admin'
            WHEN (Roles & 2) = 2 THEN 'Seller'
            WHEN (Roles & 1) = 1 THEN 'Bidder'
            ELSE 'None'
        END AS Role,
        CASE Status
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Suspended'
            WHEN 3 THEN 'Banned'
            ELSE 'Unknown'
        END AS Status,
        CreatedOnUtc AS JoinedAt,
        LastLogin
    FROM Users
    WHERE Id IN @UserIds
    ORDER BY CreatedOnUtc DESC"; // Default safe sorting for exports

    return await ExecuteResilientAsync(async connection =>
    {
        var command = new CommandDefinition(sql, new { UserIds = userIds }, cancellationToken: ct);
        var result = await connection.QueryAsync<UserDto>(command);
        
        return result.ToList().AsReadOnly();
    });
}

}