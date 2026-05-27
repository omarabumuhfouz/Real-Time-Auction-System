using MazadZone.Application.Features.Users.Commands.BulkSuspend;

namespace MazadZone.Api.Endpoints.Users;

public record BulkSuspendUsersRequest(List<UserId> UserIds, string Reason, DateTime Until);

public static class BulkSuspend
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/bulk-suspend", HandleAsync)
           // .RequireAuthorization("AdminOnly")
           .WithSummary("Bulk suspend multiple user accounts")
           .WithDescription("Temporarily suspends multiple user accounts until the specified date with a shared reason. If any single user fails validation, the entire bulk operation is aborted.")
           .Accepts<BulkSuspendUsersRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status409Conflict) // Emitted if domain rules fail (e.g. user already banned)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] BulkSuspendUsersRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new BulkSuspendUsersCommand(request.UserIds, request.Reason, request.Until), ct);
        
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}