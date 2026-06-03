using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MazadZone.Api.Endpoints.Notifications;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var notificationGroup = app.MapGroup("api/v{version:apiVersion}/notifications")
                           .WithApiVersionSet(versionSet)
                           .MapToApiVersion(1, 0)
                           .WithTags("Notifications");

        Create.MapEndpoint(notificationGroup);
        GetNotifications.MapEndpoint(notificationGroup);
        GetById.MapEndpoint(notificationGroup);
        MarkAsRead.MapEndpoint(notificationGroup);
        Delete.MapEndpoint(notificationGroup);
    }
}