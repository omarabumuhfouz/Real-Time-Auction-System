using MazadZone.Application.Features.SellerDashboard.Queries.GetOrders;
using MazadZone.Application.Features.SellerDashboard.DTOs;
using MazadZone.Infrastructure.Authentication;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using System;
using MazadZone.Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace MazadZone.Api.Endpoints.SellerDashboard;

public static class GetOrders
{
    public static void MapGetOrders(this IEndpointRouteBuilder app)
    {
        app.MapGet("orders", async (
            [AsParameters] SellerDashboardFilter filter,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken ct) =>
        {
            var userId = user.GetUserId();
            if (userId is null) return Results.Unauthorized();

            var result = await sender.Send(new GetSellerOrdersQuery(userId.Value, filter), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.TopError);
        })
        .WithName("GetSellerOrders")
        .Produces<SellerOrdersResponse>(StatusCodes.Status200OK);
    }
}
