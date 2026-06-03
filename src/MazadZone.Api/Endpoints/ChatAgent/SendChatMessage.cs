using MazadZone.Application.Features.ChatAgent.Commands.SendChatMessage;
using System.Security.Claims;

namespace MazadZone.Api.Endpoints.ChatAgent;

public record SendChatMessageRequest(string Message);

public static class SendChatMessage
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        // TODO(security): Add rate limiting to prevent abuse of the LLM endpoint (e.g., .RequireRateLimiting("chat")).
        app.MapPost("/messages", HandleAsync)
            .WithName("SendChatMessage")
            .WithOpenApi()
            .WithSummary("Send a message to the AI sales agent")
            .WithDescription("Sends a user message to the AI-powered sales assistant that answers questions about active auctions.")
            .Produces<ChatAgentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] SendChatMessageRequest? request,
        [FromServices] ISender sender,
        HttpContext httpContext,
        CancellationToken ct)
    {
        if (request is null)
        {
            return Results.BadRequest("Request body cannot be null.");
        }

        // Extract user ID from JWT claims
        var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new SendChatMessageCommand(userId, request.Message);
        var result = await sender.Send(command, ct);

        return result.Match(
            onValue: response => Results.Ok(new ChatAgentResponse(response)),
            onError: e => e.ToProblem());
    }
}

/// <summary>
/// Response wrapper for the chat agent endpoint.
/// </summary>
public record ChatAgentResponse(string Response);
