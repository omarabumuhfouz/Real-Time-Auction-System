#region  Usings
using MazadZone.Api;
using MazadZone.Api.Endpoints.Auctions;
using MazadZone.Api.Endpoints.Bidders;
using MazadZone.Api.Endpoints.Notifications;
using MazadZone.Api.Middlewares;
using MazadZone.Infrastructure.Persistence;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using MazadZone.Api.Endpoints.Orders;
using MazadZone.Api.Endpoints.Categories;
using MazadZone.Api.Endpoints.Payments;
using MazadZone.Api.Endpoints.Users;
using MazadZone.Api.Endpoints.Sellers;
using MazadZone.Api.Endpoints.SellerDashboard;
using MazadZone.Api.Endpoints.Auth;
using MazadZone.Infrastructure.RealTime.Hubs;
using Hangfire;
using MazadZone.Api.Endpoints.ChatAgent;
using MazadZone.Api.Endpoints.Disputes;
using MazadZone.Api.Endpoints.DisputeTypes;
using MazadZone.Api.Endpoints.Dashboard;
#endregion

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    Log.Information("Starting MazadZone Application.");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Explicitly permit the frontend origin
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Mandated for SignalR auth handshakes
        });
    });

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddMazadZoneServices(builder.Configuration);

    builder.Services.AddSignalR();

    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Auto apply migrations
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }

    // ==========================================
    // Middlewares Configuration
    // ==========================================

    // Https Redirection
    app.UseHttpsRedirection();

    // Correlation ID Configuration
    app.UseMiddleware<CorrelationIdMiddleware>();

    // Serilog Request Logging Configuration
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };

        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return Serilog.Events.LogEventLevel.Verbose;

            return elapsed > 500
                ? Serilog.Events.LogEventLevel.Warning
                : Serilog.Events.LogEventLevel.Information;
        };
    });

    // CORS Configuration
    app.UseCors("AllowAll");

    // Auth Configuration
    app.UseAuthentication();
    app.UseAuthorization();

    // Scalar Dashboard
    app.MapScalarApiReference();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // ==========================================
    // EndPoints and Hubs registration
    // ==========================================
    app.MapDashboardEndpoints();
    app.MapNotificationEndpoints();
    app.MapBidderEndpoints();
    app.MapOrderEndpoints();
    app.MapPaymentEndpoints();
    app.MapCategoryEndpoints();
    app.MapUserEndpoints();
    app.MapSellerEndpoints();
    app.MapSellerDashboardEndpoints();
    app.MapAuctionEndpoints();
    app.MapDisputeEndpoints();
    app.MapDisputeTypeEndpoints();
    app.MapAuthenticationEndpoints();
    app.MapChatAgentEndpoints();

    // الـ Hubs الخاصة بـ SignalR (ستعمل الآن المصادقة عليها بنجاح)
    app.MapHub<AuctionsHub>("/hubs/auctions");
    app.MapHub<NotificationsHub>("/hubs/notifications");

    // لوحة تحكم Hangfire
    app.UseHangfireDashboard("/hangfire");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}