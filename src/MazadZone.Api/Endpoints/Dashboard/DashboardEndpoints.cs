using MazadZone.Api.Endpoints.Categories;

namespace MazadZone.Api.Endpoints.Dashboard;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var versionSet = app.NewApiVersionSet()
                            .HasApiVersion(new ApiVersion(1, 0))
                            .ReportApiVersions()
                            .Build();

        // Dashboard Group
        var dashboardGroup = app.MapGroup("api/v{version:apiVersion}/dashboard")
                                .WithApiVersionSet(versionSet)
                                .MapToApiVersion(1, 0)
                                .WithTags("Dashboard Statistics");

        // Map Dashboard Actions
        GetStatistics.MapEndpoint(dashboardGroup);
        GetUserGrowthTrends.MapEndpoint(dashboardGroup);
        GetUserTrustStatistics.MapEndpoint(dashboardGroup);
        GetAuctionActivityTrends.MapEndpoint(dashboardGroup);

        GetCategoryStatistics.MapEndpoint(dashboardGroup);
        GetDisputesBreakdown.MapEndpoint(dashboardGroup);
    }
}