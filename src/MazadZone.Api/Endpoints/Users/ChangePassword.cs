using MazadZone.Api.Infrastructure.Binding;
using MazadZone.Application.Features.Users.Commands.ChangePassword;

namespace MazadZone.Api.Endpoints.Users;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword);

public static class ChangePassword
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/password", HandleAsync)
           .RequireAuthorization()
           .WithSummary("Change user password")
           .WithDescription("Updates the password for the currently authenticated user. The request requires the user to provide their current password for verification. Returns a 400 Bad Request if the new passwords do not match or fail to meet complexity requirements.")
           .Accepts<ChangePasswordRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest) // e.g., Weak password or passwords do not match
           .ProducesProblem(StatusCodes.Status401Unauthorized) // Missing token OR incorrect current password
           .ProducesProblem(StatusCodes.Status404NotFound) // Bound user no longer exists in the DB
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ChangePasswordRequest request,
        BoundUserId boundUserId,
        [FromServices]ISender sender,
        CancellationToken ct)
    {
        var command = new ChangePasswordCommand(
            boundUserId.Value, 
            request.CurrentPassword, 
            request.NewPassword, 
            request.ConfirmNewPassword);

        var result = await sender.Send(command, ct);
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}