namespace Tests.Application.Features.Users.Queries.GetUsers;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;
using MazadZone.Application.Features.Users.Queries.GetUsers;
using MazadZone.Application.Features.Users.DTOs;
using MazadZone.Application.Services;
using MazadZone.Application.Common.Paging;

public class GetUsersQueryHandlerTests : UserBaseTest<GetUsersQueryHandler>
{
    [Fact]
    public async Task Handle_ReturnsUsers_WhenDataExists()
    {
        // Arrange
        var filterParams = new UserFilterParams { PageNumber = 1, PageSize = 10 };
        var query = new GetUsersQuery(filterParams);

        var mockUsers = new List<UserDto>
        {
            new UserDto(System.Guid.NewGuid(), "John Doe", "john@example.com", "+123", "User", "Active", System.DateTime.UtcNow, System.DateTime.UtcNow)
        };
        var pagedList = new PagedList<UserDto>(mockUsers, 1, 10, 1);

        _userQueries.GetUsersAsync(filterParams, Arg.Any<CancellationToken>())
            .Returns(pagedList);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Items.Count.ShouldBe(1);
        result.Value.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_ReturnsEmptyPagedList_WhenRepositoryReturnsNull()
    {
        // Arrange
        var filterParams = new UserFilterParams { PageNumber = 1, PageSize = 10 };
        var query = new GetUsersQuery(filterParams);

        // Simulate repository returning null unexpectedly
        _userQueries.GetUsersAsync(filterParams, Arg.Any<CancellationToken>())
            .Returns((PagedList<UserDto>?)null);

        // Act
        var result = await Handler.Handle(query, default);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        
        // Should safely fallback to the empty list
        result.Value.Items.ShouldBeEmpty();
        result.Value.TotalCount.ShouldBe(0);
    }
}