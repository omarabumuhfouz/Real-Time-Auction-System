namespace Tests.Application.Features.Users.Queries.ExportUsers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Queries.ExportUsers;
using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Application.Common.Paging;

public class ExportUsersQueryHandlerTests : UserBaseTest<ExportUsersQueryHandler>
{
    [Fact]
    public async Task Handle_ForcesIsExportToTrue_AndReturnsOnlyHeadersIfNoData()
    {
        // Arrange
        // Create filter params where IsExport is explicitly FALSE
        var filterParams = new UserFilterParams { IsExport = false, PageNumber = 1, PageSize = 10 };
        var command = new ExportUsersQuery(filterParams);

        var emptyPagedList = new PagedList<UserDto>(new List<UserDto>(), 0, 1, 10);

        // We explicitly mock the repository to ONLY return data if IsExport == true
        _userQueries.GetUsersAsync(Arg.Is<UserFilterParams>(p => p.IsExport == true), Arg.Any<CancellationToken>())
            .Returns(emptyPagedList);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ContentType.ShouldBe("text/csv");
        result.Value.FileName.ShouldStartWith("Users_Export_");
        result.Value.FileName.ShouldEndWith(".csv");

        var csvContent = Encoding.UTF8.GetString(result.Value.FileContents);
        var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        lines.Length.ShouldBe(1);
        lines.First().ShouldBe("Id,Full Name,Email,Phone Number,Role,Status,Joined At,Last Login");
        
        // Double-check that our dependency was called with the overridden parameter
        await _userQueries.Received(1).GetUsersAsync(Arg.Is<UserFilterParams>(p => p.IsExport == true), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UsersFound_ReturnsFormattedCsvWithData()
    {
        // Arrange
        var filterParams = new UserFilterParams { SearchTerm = "John" }; // Optional other filters
        var command = new ExportUsersQuery(filterParams);

        var mockDate = new DateTime(2023, 10, 1, 12, 30, 0);

        var mockUsers = new List<UserDto>
        {
            new UserDto(
                Guid.NewGuid(), 
                "John Doe",
                "john@example.com",
                "+123456789",
                "Admin",
                "Active",
                mockDate,
                mockDate,
                "Verified",       // VerificationStatus
                null,             // NationalId
                null,             // ExtractedFullName
                null              // VerificationRejectionReason
            )
        };

        var populatedPagedList = new PagedList<UserDto>(mockUsers, 1, 1, 10);

        _userQueries.GetUsersAsync(Arg.Any<UserFilterParams>(), Arg.Any<CancellationToken>())
            .Returns(populatedPagedList);

        // Act
        var result = await Handler.Handle(command, default); 

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        var csvContent = Encoding.UTF8.GetString(result.Value.FileContents);
        var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Verify we have exactly 2 rows (1 Header + 1 Data Row)
        lines.Length.ShouldBe(2); 
        
        // Verify Header Row
        lines.First().ShouldBe("Id,Full Name,Email,Phone Number,Role,Status,Joined At,Last Login");
        
        // Verify Data Row formatting (quotes, dates, etc.)
        var expectedDataRow = $"{mockUsers.First().Id},\"John Doe\",\"john@example.com\",\"+123456789\",Admin,Active,2023-10-01 12:30:00,2023-10-01 12:30:00";
        lines.Last().ShouldBe(expectedDataRow);
    }
}