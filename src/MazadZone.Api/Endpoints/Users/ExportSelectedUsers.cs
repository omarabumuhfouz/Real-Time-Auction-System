using MazadZone.Application.Features.Users.Queries.ExportSelectedUsers;

namespace MazadZone.Api.Endpoints.Users;

public static class ExportSelectedUsers
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/export/selected", HandleAsync)
              .RequireAuthorization(Policies.AdminOnly)
           .WithSummary("Export specific users by IDs to CSV")
           .WithDescription("Accepts a JSON array of User IDs and generates a downloaded CSV file.")
           .Produces(StatusCodes.Status200OK, contentType: "text/csv")
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] List<Guid> selectedIds,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new ExportSelectedUsersQuery(selectedIds), ct);

        return result.Match(
            fileResponse => Results.File(
                fileResponse.FileContents, 
                fileResponse.ContentType, 
                fileResponse.FileName),
            error => error.ToProblem());
    }
}