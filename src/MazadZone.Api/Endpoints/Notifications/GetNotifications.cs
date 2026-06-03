using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Queries.GetNotifications;
using MazadZone.Application.Features.Notifications.Queries.DTOs;
using MazadZone.Api.Contracts.Notifications;
using MazadZone.Api.Extensions;

namespace MazadZone.Api.Endpoints.Notifications;

public static class GetNotifications
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", HandleAsync)
            .WithSummary("Gets User Notifications")
            .WithDescription("Retrieves paginated notifications for a specific user.")
            .Produces<NotificationsListDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetNotificationsRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        if (request is null || request.PageNumber < 1 || request.PageSize < 1)
        {
            return Results.BadRequest("Invalid pagination parameters.");
        }

        try
        {
            var query = new GetNotificationsQuery(request.UserId, request.PageNumber, request.PageSize);

            var result = await sender.Send(query, cancellationToken);

            return result.Match(
                success => Results.Ok(success),
                errors => errors.ToProblem()
            );
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.ToString(), statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
