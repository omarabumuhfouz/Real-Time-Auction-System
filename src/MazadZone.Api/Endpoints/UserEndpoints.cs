using AuthService.Api.Contracts.Users;
using MazadZone.Api.Contracts.Users;
using MazadZone.Application.Features.Users.Commands.Activate;
using MazadZone.Application.Features.Users.Commands.Ban;
using MazadZone.Application.Features.Users.Commands.ChangeEmail;
using MazadZone.Application.Features.Users.Commands.ChangePassword;
using MazadZone.Application.Features.Users.Commands.Suspend;
using MazadZone.Domain.Users.ValueObjects;
using MechanicShop.Api.Extensions;

namespace MazadZone.Api.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                                    .HasApiVersion(new ApiVersion(1, 0))
                                    .ReportApiVersions()
                                    .Build();

        var userGroup = app.MapGroup("api/v{version:apiVersion}/users")
                       .WithApiVersionSet(versionSet)
                       .MapToApiVersion(1, 0)
                       .WithTags("Users Queries");

        var authApi = app.MapGroup("/api/auth")
            .WithTags("Auth")
            .WithOpenApi();

        // --- User Management Endpoints (Admin/Internal) ---
        userGroup.MapPost("/{id:guid}/activate", Activate)
            .RequireAuthorization("AdminOnly") // Applied the Admin policy
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Activate a user account");

// Added the Ban Endpoint
        userGroup.MapPost("/{id:guid}/ban", Ban)
            .RequireAuthorization("AdminOnly")
            .Accepts<BanUserRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Ban a user account for a specific reason");

    userGroup.MapPost("/{id:guid}/suspend", Suspend)
            .RequireAuthorization("AdminOnly")
            .Accepts<SuspendUserRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Suspend a user account until a specific date");

        authApi.MapPost("/change-password", ChangePassword)
            // .RequireAuthorization(AuthConstants.Policies.Shared)
            .Accepts<ChangePasswordRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Change user password");

        authApi.MapPut("/users/email", ChangeEmail)
            // .RequireAuthorization(AuthConstants.Policies.Shared)
            .Accepts<ChangeEmailRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Change user email");


        return authApi;
    }

    private static async Task<IResult> Suspend(
            Guid id,
            [FromBody] SuspendUserRequest request,
            ISender sender,
            CancellationToken ct)
    {
        var command = new SuspendUserCommand(UserId.Load(id), request.Until);

        var result = await sender.Send(command, ct);

        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }

    private static async Task<IResult> Ban(
            UserId userId,
            [FromBody] BanUserRequest request,
            ISender sender,
            CancellationToken ct)
    {
        var command = new BanUserCommand(userId, request.Reason);
        var result = await sender.Send(command, ct);

        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }

    private static async Task<IResult> Activate(
        UserId userId,
        ISender sender,
        CancellationToken ct)
    {
        // We use the ID from the URL route, not the caller's token
        var result = await sender.Send(new ActivateUserCommand(userId), ct);

        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }

    private static async Task<IResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        [FromServices] IHttpContextAccessor context,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        Guid userId = context.HttpContext?.User.GetUserId() ?? Guid.Empty;
        var command = new ChangePasswordCommand(UserId.Load(userId), request.CurrentPassword, request.NewPassword, request.ConfirmNewPassword);
        var result = await sender.Send(command, ct);

        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }

    private static async Task<IResult> ChangeEmail(
        [FromBody] ChangeEmailRequest request,
        [FromServices] IHttpContextAccessor context,
        [FromServices] ISender sender,
        CancellationToken ct
        )
    {
        Guid userId = context.HttpContext?.User.GetUserId() ?? Guid.Empty;
        var command = new ChangeEmailCommand(UserId.Load(userId), request.NewEmail);
        var result = await sender.Send(command, ct);

        return result.Match(
            _ => Results.NoContent(),
             e => e.ToProblem());
    }
}