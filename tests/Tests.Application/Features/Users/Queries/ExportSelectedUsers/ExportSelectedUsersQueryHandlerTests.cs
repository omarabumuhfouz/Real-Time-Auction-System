namespace Tests.Application.Features.Users.Queries.ExportSelectedUsers;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Queries.ExportSelectedUsers;
using MazadZone.Application.Features.Users.DTOs;

public class ExportSelectedUsersQueryHandlerTests : UserBaseTest<ExportSelectedUsersQueryHandler>
{
    [Fact]
    public async Task Handle_NoUsersFound_ReturnsCsvWithOnlyHeaders()
    {
        // Arrange
        // Assuming UserId or Guid is used in the query
        var command = new ExportSelectedUsersQuery(new List<Guid> { Guid.NewGuid() });

        // Mock repo to return an empty list
        _userQueries.ExportSelectedUsersAsync(command.SelectedUserIds, Arg.Any<CancellationToken>())
            .Returns(new List<UserDto>()); // Assuming UserExportDto is the return type

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ContentType.ShouldBe("text/csv");
        result.Value.FileName.ShouldStartWith("Selected_Users_Export_");

        // Decode the byte array back to a string to verify contents
        var csvContent = Encoding.UTF8.GetString(result.Value.FileContents);

        // Should only contain the header row and a newline
        csvContent.Trim().ShouldBe("Id,Full Name,Email,Phone Number,Role,Status,Joined At,Last Login");
    }

    [Fact]
    public async Task Handle_UsersFound_ReturnsFormattedCsvWithData()
    {
        // Arrange
        var command = new ExportSelectedUsersQuery(new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

        var mockDate = new DateTime(2023, 10, 1, 12, 30, 0);

        var mockUsers = new List<UserDto>
    {
        new UserDto(
            command.SelectedUserIds.First(), // Safely get the first Guid
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

        // Mock the repository
        _userQueries.ExportSelectedUsersAsync(command.SelectedUserIds, Arg.Any<CancellationToken>())
            .Returns(mockUsers);

        // Act
        var result = await Handler.Handle(command, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var csvContent = Encoding.UTF8.GetString(result.Value.FileContents);

        // Safest cross-platform split for CSV lines
        var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        // Verify we have exactly 2 rows (1 Header + 1 Data Row)
        lines.Length.ShouldBe(2);

        // Verify Header Row
        lines.First().ShouldBe("Id,Full Name,Email,Phone Number,Role,Status,Joined At,Last Login");

        // Verify Data Row (safely using .First().Id from the mock list!)
        var expectedDataRow = $"{mockUsers.First().Id},\"John Doe\",\"john@example.com\",\"+123456789\",Admin,Active,2023-10-01 12:30:00,2023-10-01 12:30:00";
        lines.Last().ShouldBe(expectedDataRow);
    }
}