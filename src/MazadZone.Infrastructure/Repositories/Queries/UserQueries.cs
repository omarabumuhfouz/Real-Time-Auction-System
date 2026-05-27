using Dapper;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Features.Bidders.DTOs;
using MazadZone.Application.Features.Users.Queries.GetProfileSettings;
using MazadZone.Application.Services;
using MazadZone.Domain.Primitives.Results;
using MazadZone.Domain.Shared.ValueObjects;
using MazadZone.Domain.Users.ValueObjects;
using Polly;

namespace MazadZone.Infrastructure.Repositories.Queries;

public class UserQueries : ResilientRepository, IUserQueries
{
    public UserQueries(ISqlConnectionFactory sqlFactory, IAsyncPolicy resiliencePolicy): base(sqlFactory, resiliencePolicy){}

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


}