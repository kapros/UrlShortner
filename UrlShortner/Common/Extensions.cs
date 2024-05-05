using System.Reflection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using UrlShortner.DataAccess;
using UrlShortner.Jobs;
using UrlShortner.Shorten;

namespace UrlShortner.Common;

public static class Extensions
{
    public static WebApplicationBuilder RegisterDevDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<UrlShortenerDbContext>(opt => opt.UseInMemoryDatabase("UrlShortener"));
        // can also seed the DB later
        builder.Services.AddMemoryCache();
        return builder;
    }

    public static WebApplicationBuilder RegisterUrlServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRandomizer, Randomizer>();
        builder.Services.AddScoped<UrlShorteningService>();
        builder.Services.AddScoped<IUrlShorteningService>(
            x =>
            new CachedUrlShorteningService(x.GetRequiredService<UrlShorteningService>(), x.GetRequiredService<IMemoryCache>()));
        return builder;
    }

    /// <summary>
    /// Adds config and service for deleting old links.
    /// </summary>
    /// <remarks>DOES NOT WORK WITH IN-MEMORY DB</remarks>
    /// <see cref="https://github.com/dotnet/efcore/issues/30185"/>
    public static WebApplicationBuilder RegisterStaleUrlsDeletingService(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration.Get<StaleConfigurationDeletingServiceSettings>();
        builder.Services.AddSingleton(config);
        builder.Services.AddHostedService<StaleUrlsDeletingJob>();
        return builder;
    }

    /// <summary>
    /// Adds other non domain specific services like:
    /// - response compression
    /// </summary>
    public static WebApplicationBuilder AddNonDomainServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(opts =>
        {
            opts.EnableForHttps = true;
            opts.Providers.Add<BrotliCompressionProvider>();
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes;
        });
        builder.Services.Configure<BrotliCompressionProviderOptions>(opts =>
        {
            opts.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        builder.Host.UseSerilog(logger);
        return builder;
    }

    public static WebApplicationBuilder RegisterHandlers(this WebApplicationBuilder builder)
    {
        builder.RegisterCommandHandlers();
        builder.RegisterQueryHandlers();
        return builder;
    }

    public static WebApplicationBuilder RegisterCommandHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ShortUrlCommandHandler>();
        return builder;
    }

    public static WebApplicationBuilder RegisterQueryHandlers(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<GetShortenedUrlQueryHandler>();
        return builder;
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => 
            type is { IsAbstract: false, IsInterface: false } && 
            type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();
        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null, int? versionsToMap = null)
    {
        var endpoints = app.Services
            .GetRequiredService<IEnumerable<IEndpoint>>();

        var builder =
            routeGroupBuilder is null ? app as IEndpointRouteBuilder : routeGroupBuilder;

        foreach (var endpoint in endpoints)
        {
            if (!versionsToMap.HasValue || endpoint.ApiVersions.Contains(versionsToMap.Value))
            {
                endpoint.MapEndpoint(builder)
                    .WithName(endpoint.EndpointName)
                    .WithTags(endpoint.EndpointTag)
                    .WithOpenApi();
            }
        }

        return app;
    }
}

/*
    https://www.milanjovanovic.tech/blog/lightweight-in-memory-message-bus-using-dotnet-channels
    https://www.milanjovanovic.tech/blog/using-masstransit-with-rabbitmq-and-azure-service-bus
    https://www.milanjovanovic.tech/blog/value-objects-in-dotnet-ddd-fundamentals
*/
