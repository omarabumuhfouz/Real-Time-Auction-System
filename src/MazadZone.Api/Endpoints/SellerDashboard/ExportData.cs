using MazadZone.Application.Features.SellerDashboard.Queries.ExportData;
using MazadZone.Application.Features.SellerDashboard.DTOs;
using MazadZone.Infrastructure.Authentication;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System;
using MazadZone.Domain.Users.ValueObjects;

namespace MazadZone.Api.Endpoints.SellerDashboard;

public static class ExportData
{
    public static void MapExportData(this IEndpointRouteBuilder app)
    {
        app.MapGet("export", async (
            string type,
            [AsParameters] SellerDashboardFilter filter,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            if (userId is null) return Results.Unauthorized();

            var result = await sender.Send(new ExportSellerDataQuery(userId.Value, type, filter), ct);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.TopError);
            }

            return Results.File(result.Value, "text/csv", $"seller_{type}_{DateTime.UtcNow:yyyyMMdd}.csv");
        })
        .WithName("ExportSellerData")
        .WithSummary("Export Seller Data")
        .WithDescription("Export Seller Data")
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status200OK, contentType: "text/csv");
    }
}
