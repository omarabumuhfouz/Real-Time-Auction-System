using MazadZone.Api.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MazadZone.Api.Endpoints.SellerDashboard;

public static class SellerDashboardEndpoints
{
    public static void MapSellerDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/seller-dashboard")
            .WithTags("Seller Dashboard")
            .RequireAuthorization(Policies.SellerOnly);

        group.MapGetAuctions();
        group.MapGetOrders();
        group.MapGetFinancials();
        group.MapExportData();
    }
}
