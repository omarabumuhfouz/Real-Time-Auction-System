namespace MazadZone.Api.Endpoints.ChatAgent;

public static class ChatAgentEndpoints
{
    public static void MapChatAgentEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        var chatGroup = app.MapGroup("api/v{version:apiVersion}/chat")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1, 0)
            .WithTags("ChatAgent");

        SendChatMessage.MapEndpoint(chatGroup);
    }
}
