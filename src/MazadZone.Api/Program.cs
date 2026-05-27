
using MazadZone.Api;
using MazadZone.Api.Endpoints;
using MazadZone.Api.Endpoints.Auctions;
using MazadZone.Api.Endpoints.Bidders;
using MazadZone.Api.Endpoints.Notifications;
using MazadZone.Api.Middlewares;
using MazadZone.Infrastructure.Persistence;
using MazadZone.Infrastructure.Persistence.Seeding;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using MazadZone.Api.Endpoints.Orders;
using MazadZone.Api.Endpoints.Categories;
using MazadZone.Api.Endpoints.Payments;
using MazadZone.Api.Endpoints.Users;
using MazadZone.Api.Endpoints.Sellers;
using MazadZone.Api.Endpoints.Auth;
using MazadZone.Infrastructure.RealTime.Hubs;
using Hangfire;
using MazadZone.Api.Endpoints.ChatAgent;
using MazadZone.Api.Endpoints.Disputes;
using MazadZone.Api.Endpoints.DisputeTypes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    Log.Information("Starting MazadZone Application.");


    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        // Define a custom policy named "AllowAll"
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()    // Allows access from any domain
                  .AllowAnyHeader()    // Allows any headers (e.g., Content-Type, Authorization)
                  .AllowAnyMethod();   // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
        });
    });


    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    builder.Services.AddMazadZoneServices(builder.Configuration);

    builder.Services.AddSignalR();

    builder.Services.AddOpenApi();



    var app = builder.Build();

    // Apply pending migrations automatically on startup
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // This will create the DB and apply all migrations if they don't exist
        dbContext.Database.Migrate();

        // Fix any items with Status = 0 (due to migration default values)
        dbContext.Database.ExecuteSqlRaw("UPDATE Items SET Status = 1 WHERE Status = 0");

        // Optional: You can also call a DatabaseSeeder here to insert dummy data!
        // var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        // await seeder.SeedAsync();
    }
    app.UseCors("AllowAll");
    //Map Endpoints
    app.MapNotificationEndpoints();
    app.MapBidderEndpoints();
    app.MapOrderEndpoints();
    app.MapPaymentEndpoints();
    app.MapCategoryEndpoints();
    app.MapUserEndpoints();
    app.MapSellerEndpoints();
    app.MapAuctionEndpoints();
    app.MapDisputeEndpoints();
    app.MapDisputeTypeEndpoints();
    app.MapAuthenticationEndpoints();
    app.MapChatAgentEndpoints();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

    }

    // #region  Seed database
    // if (app.Environment.IsDevelopment())
    // {
    //     // Create a temporary DI scope
    //     using var scope = app.Services.CreateScope();

    //     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //     var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();

    //     try
    //     {
    //         Console.WriteLine("Applying database migrations...");
    //         // 1. Ensure the database exists and schema is up to date
    //         await dbContext.Database.MigrateAsync();

    //         Console.WriteLine("Seeding mock data for MazadZone...");
    //         // 2. Execute your MasterDataSeeder
    //         await seeder.SeedAsync();

    //         Console.WriteLine(" Database seeded successfully!");
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($" Error during database seeding: {ex}");
    //     }
    // }

    // #endregion
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };

        // Exclude health check endpoints from request logs
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return Serilog.Events.LogEventLevel.Verbose;

            return elapsed > 500
                ? Serilog.Events.LogEventLevel.Warning
                : Serilog.Events.LogEventLevel.Information;
        };
    });
    app.MapScalarApiReference();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    //Map Hubs
    app.MapHub<AuctionsHub>("/hubs/auctions");
    app.MapHub<NotificationsHub>("/hubs/notifications");

    //Hangfire Dashboard
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
