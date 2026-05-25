using MazadZone.Api.Constants;
using MazadZone.Api.OpenApi.Transformers;
using MazadZone.Application;
using MazadZone.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace MazadZone.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddMazadZoneServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true; // This registers the missing IReportApiVersions service
        });

        services.AddInfrastructureServices(configuration)
        .AddApplicationServices()
        .AddApiDocumentation()
        .AddJwtAuthentication(configuration);

        return services;
    }

private static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {

        services.AddOpenApi("v1", options =>
        {
            // Use transformer class
            options.AddDocumentTransformer<DefaultInfoTransformer>();


            // Security Scheme config
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        // Now you can access 'builder.Host'
        builder.Host.UseSerilog((context, loggerCongiguration) =>
            loggerCongiguration.ReadFrom.Configuration(context.Configuration));

        return builder;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {

        // Register your provider as a Singleton so the IMemoryCache works correctly
        services.AddSingleton<DatabaseKeyProvider>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(); // Kept empty, options configured below

        // Inject the DatabaseKeyProvider into JwtBearerOptions
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<DatabaseKeyProvider>((options, keyProvider) =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    // Route the key resolution to our new DatabaseKeyProvider
                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        return keyProvider.GetKeys(token, securityToken, kid, validationParameters);
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Policies.BidderOnly, policy => policy.RequireRole(Roles.Bidder));
            options.AddPolicy(Policies.BidderAndSeller, policy => policy.RequireRole(Roles.Bidder, Roles.Seller));
            options.AddPolicy(Policies.Shared, policy => policy.RequireRole(Roles.Admin, Roles.Bidder, Roles.Seller));
        });

        return services;
    }

}

