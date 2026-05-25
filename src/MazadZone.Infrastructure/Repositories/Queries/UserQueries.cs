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

    public Task<Result<Address>> GetAddressByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
                -- Base User Data
                u.Id,
                u.FirstName + ' ' + u.SecondName + ' ' + u.ThirdName + ' ' + u.LastName AS FullName,
                u.Email,
                u.PhoneNumber,
                b.NationalId
            FROM Users u
            LEFT JOIN Bidders b ON u.Id = b.Id
            WHERE u.Id = @UserId;

            SELECT 
                a.City,
                a.Street,
                a.Building,
                a.Landmark
            FROM Addresses a
            WHERE a.UserId = @UserId;
            ";

        return await ExecuteResilientAsync(async connection =>
        {
            using var multi = await connection.QueryMultipleAsync(sql, new { UserId = userId.Value });

            var user = await multi.ReadFirstOrDefaultAsync<UserBaseProfile>();
            if (user is null) return null;

            var addresses = (await multi.ReadAsync<AddressDto>()).ToList();
            if (!addresses.Any()) return null;

            return new ProfileSettingsResponse(
                user.Id,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.NationalId,
                addresses
            );
        });
    }

    private record UserBaseProfile(Guid Id, string FullName, string Email, string PhoneNumber, string NationalId);

}