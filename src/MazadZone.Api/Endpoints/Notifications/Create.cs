using Microsoft.AspNetCore.Mvc;
using MediatR;
using MazadZone.Application.Features.Notifications.Commands.CreateNotification;
using MazadZone.Api.Contracts.Notifications;
using MazadZone.Api.Extensions;

namespace MazadZone.Api.Endpoints.Notifications;

public static class Create
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/", HandleAsync)
            .WithName("CreateNotification")
            .WithOpenApi()
            .WithSummary("Creates a new Notification")
            .WithDescription("Creates a new notification for a specified user with a title and message body. This is typically called by internal system processes. **Requires Admin role.**")
            .RequireAuthorization(Policies.AdminOnly)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateNotificationRequest? request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        try
        {
            var command = new CreateNotificationCommand(request.UserId, MazadZone.Application.Features.Notifications.Enums.NotificationMethods.ReceiveNotification, request.Title, request.Message);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(
                success => Results.Created($"/api/notifications/{success.Value}", success.Value),
                errors => errors.ToProblem()
            );
        }
        catch (Exception)
        {
            return Results.Problem("An unexpected error occurred processing the notification.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
