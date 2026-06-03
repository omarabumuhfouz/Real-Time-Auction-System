using MazadZone.Application.Features.Users.Commands.BulkBan;

namespace MazadZone.Api.Endpoints.Users;

public record BulkBanUsersRequest(List<UserId> UserIds, string Reason);

public static class BulkBan
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/bulk-ban", HandleAsync)
           .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Bulk ban multiple user accounts")
           .WithDescription("Suspends multiple user accounts simultaneously with a shared reason. If any single user fails validation, the entire bulk operation is aborted.")
           .Accepts<BulkBanUsersRequest>("application/json")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] BulkBanUsersRequest request,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new BulkBanUsersCommand(request.UserIds, request.Reason), ct);
        
        return result.Match(
            _ => Results.NoContent(),
            e => e.ToProblem());
    }
}