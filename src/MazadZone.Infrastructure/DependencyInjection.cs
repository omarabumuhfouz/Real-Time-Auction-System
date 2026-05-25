using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Hangfire;
using MazadZone.Application.Common.Interfaces;
using MazadZone.Application.Services;
using MazadZone.Domain.Shared.Interfaces;
using MazadZone.Infrastructure.Caching;
using MazadZone.Infrastructure.Configuration;
using MazadZone.Infrastructure.Outbox;
using MazadZone.Infrastructure.Persistence;
using MazadZone.Infrastructure.Persistence.Interceptors;
using AuthService.Infrastructure.Backgrounds; 

namespace MazadZone.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOutboxPattern(configuration)
            .AddDatabase(configuration)
            .AddPollyPolicies(configuration)
            .AddServiceScanning()
            .AddSignalRServices()
            .AddHangfireServices(configuration)
            .AddCachingServices(configuration)
            .AddBackgroundServices();




        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var resilienceOptions = new ResilienceOptions();
        configuration.GetSection(ResilienceOptions.SectionName).Bind(resilienceOptions);

        var connectionString = configuration.GetConnectionString("MazadZoneDb")
            ?? throw new InvalidOperationException("Default DB connection string not found.");

        // 1. Register Interceptors
        services.AddSingleton<InsertOutboxMessagesInterceptor>();
        services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

        // 2. Register EF Core
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<InsertOutboxMessagesInterceptor>();
            var auditableInterceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: resilienceOptions.RetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(resilienceOptions.MaxDelaySeconds),
                    errorNumbersToAdd: null
                );

            })
                   .AddInterceptors(interceptor, auditableInterceptor);
        });

        // 3. Register Dapper
        services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(connectionString));

        return services;
    }

    private static IServiceCollection AddOutboxPattern(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));
        services.AddHostedService<ProcessOutboxMessagesJob>();

        return services;
    }

    private static IServiceCollection AddPollyPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ResilienceOptions();
        configuration.GetSection(ResilienceOptions.SectionName).Bind(options);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(options.RetryCount, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(options.BaseDelaySeconds, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    // Log logic here
                });

        services.AddSingleton<IAsyncPolicy>(retryPolicy);

        return services;
    }

   private static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMemoryCache();
services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            // Optional: Adds a prefix to all keys so you don't clash with other apps sharing the same Redis instance
            options.InstanceName = "MazadZone_"; 
        });
services.AddScoped<ICacheService, RedisCacheService>();
        return services;
    }

    public static IServiceCollection AddServiceScanning(this IServiceCollection services)
    {
        var infrastructureAssembly = typeof(DependencyInjection).Assembly;
        var applicationAssembly = typeof(MazadZone.Application.DependencyInjection).Assembly;
        var domainAssembly = typeof(MazadZone.Domain.Categories.ICategoryDomainService).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(infrastructureAssembly, applicationAssembly, domainAssembly)
            // Register Scoped Services
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            // Register Transient Services
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            // Register Singleton Services
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );

        return services;
    }

    public static IServiceCollection AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalRCore();
        return services;
    }

    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MazadZoneDb");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString));

        services.AddHangfireServer();

        // services.AddScoped<IOrderJobScheduler, HangfireOrderJobScheduler>();
        // services.AddScoped<IAuctionJobScheduler, HangfireAuctionJobScheduler>();

        return services;
    }

    // public static IServiceCollection AddPaymentService(this IServiceCollection services)
    // {
    //     // تسجيل الخدمة الوهمية وتجاهل خدمة Stripe نهائياً
    //     services.AddScoped<IPaymentService, PaymentService>();
    //     return services;
    // }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {

        services.AddHostedService<KeyRotationService>();
        return services;
    }


}