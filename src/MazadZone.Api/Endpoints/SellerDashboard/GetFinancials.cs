using MazadZone.Application.Features.SellerDashboard.Queries.GetFinancials;
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

public static class GetFinancials
{
    public static void MapGetFinancials(this IEndpointRouteBuilder app)
    {
        app.MapGet("financials", async (
            [AsParameters] SellerDashboardFilter filter,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            if (userId is null) return Results.Unauthorized();

            var result = await sender.Send(new GetSellerFinancialsQuery(userId.Value, filter), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.TopError);
        })
        .WithName("GetSellerFinancials")
        .WithSummary("Get Seller Financials")
        .WithDescription("Get Seller Financials")
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .Produces<SellerFinancialsResponse>(StatusCodes.Status200OK);
    }
}
