using System.Text;
using Dapper;
using MazadZone.Application.Common.Interfaces;
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
                DefaultShippingAddress_City AS City,
                DefaultShippingAddress_Street AS Street,
                DefaultShippingAddress_Building AS Building,
                DefaultShippingAddress_Landmark AS Landmark
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

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(UserFilterParams filter, CancellationToken ct)
{
    // 1. Base Query
    var sqlBuilder = new StringBuilder(@"
    SELECT 
        Id,
        FirstName + ' ' + LastName AS FullName,
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
    WHERE 1 = 1 ");

    var parameters = new DynamicParameters();

    // 2. Apply Filtering
    // IF Specific IDs are provided, ignore all other text/date filters
    if (filter.SelectedUserIds != null && filter.SelectedUserIds.Any())
    {
        sqlBuilder.Append(" AND Id IN @Ids ");
        parameters.Add("Ids", filter.SelectedUserIds);
    }
    else 
    {
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            sqlBuilder.Append(@" 
            AND (FirstName LIKE @Search 
                 OR LastName LIKE @Search 
                 OR Email LIKE @Search 
                 OR PhoneNumber LIKE @Search)");

            parameters.Add("Search", $"%{filter.SearchTerm}%");
        }

        if (filter.JoinedDate.HasValue)
        {
            // PRO TIP: This is SARGable (Index-friendly) instead of using CAST()
            var startDate = filter.JoinedDate.Value.Date;
            var endDate = startDate.AddDays(1);
            
            sqlBuilder.Append(" AND CreatedOnUtc >= @StartDate AND CreatedOnUtc < @EndDate");
            parameters.Add("StartDate", startDate);
            parameters.Add("EndDate", endDate);
        }
    }

    // 3. Apply Safe Sorting (Prevent SQL Injection)
    var sortColumn = filter.SortBy?.ToLower() switch
    {
        "fullname" => "FullName",
        "joineddate" => "JoinedAt",
        "lastlogin" => "LastLogin",
        "role" => "Role",
        "status" => "Status",
        _ => "JoinedAt"
    };

    var sortDirection = filter.IsAsc ? "ASC" : "DESC";
    sqlBuilder.Append($" ORDER BY {sortColumn} {sortDirection}");

    // 4. Apply Pagination (Skip if Exporting)
    if (!filter.IsExport)
    {
        sqlBuilder.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
        parameters.Add("Offset", (filter.PageNumber - 1) * filter.PageSize);
        parameters.Add("PageSize", filter.PageSize);
    }

    // 5. Execute
    return await ExecuteResilientAsync(async connection =>
    {
        // Wrap the query in CommandDefinition to support CancellationToken
        var command = new CommandDefinition(
            commandText: sqlBuilder.ToString(),
            parameters: parameters,
            cancellationToken: ct);

        var result = await connection.QueryAsync<UserDto>(command);

        return result.ToList().AsReadOnly();
    });
}
}