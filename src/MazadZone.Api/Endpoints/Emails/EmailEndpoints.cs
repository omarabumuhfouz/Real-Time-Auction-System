namespace MazadZone.Api.Endpoints.Emails;

public static class EmailEndpoints
{
    public static void MapEmailEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        var emailGroup = app.MapGroup("api/v{version:apiVersion}/emails")
                            .WithApiVersionSet(versionSet)
                            .MapToApiVersion(1, 0)
                            .WithTags("Email Management");

        // Register your endpoints here
        NotifyUser.MapEndpoint(emailGroup);
    }
}