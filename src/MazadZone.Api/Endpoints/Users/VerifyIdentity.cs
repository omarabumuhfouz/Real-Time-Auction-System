using MazadZone.Application.Features.Users.Commands.VerifyIdentity;
using MazadZone.Domain.Users.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MazadZone.Api.Endpoints.Users;

public static class VerifyIdentity
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/verify-identity", HandleAsync)
           .RequireAuthorization() // Restrict to authenticated users
           .DisableAntiforgery()   // Disable ASP.NET Core automatic form anti-forgery check
           .WithSummary("Verify user identity using ID card image OCR")
           .WithDescription("Accepts an uploaded image file of the user's national identity card, performs OCR extraction, and verifies identity.")
           .Produces(StatusCodes.Status204NoContent)
           .ProducesValidationProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .ProducesProblem(StatusCodes.Status404NotFound)
           .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UserId id,
        IFormFile file,
        [FromServices] ISender sender,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return Results.BadRequest("An image file of the identity card is required.");
        }

        // Enforce 5MB upload size limit to prevent Buffer Overflow or DoS attacks
        if (file.Length > 5 * 1024 * 1024)
        {
            return Results.BadRequest("File size must not exceed 5MB.");
        }

        // Validate allowed image formats (allowlist)
        var allowedExtensions = new[] { ".png", ".jpg", ".jpeg" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return Results.BadRequest("Only JPG, JPEG, and PNG images are permitted.");
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, ct);
        var imageBytes = memoryStream.ToArray();

        var command = new VerifyIdentityCommand(id, imageBytes);
        var result = await sender.Send(command, ct);

        return result.Match(
            _ => Results.NoContent(),
            error => error.ToProblem());
    }
}
