using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PaperApi;

/// <summary>
/// Dependency injection helpers for the PaperAPI client.
/// </summary>
public static class PaperApiServiceCollectionExtensions
{
    private const string DefaultSectionName = "PaperApi";

    /// <summary>
    /// Registers <see cref="IPaperApiClient"/> with configuration binding from the given section (defaults to "PaperApi").
    /// </summary>
    public static IServiceCollection AddPaperApiClient(this IServiceCollection services, IConfiguration configuration, string sectionName = DefaultSectionName)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        services.AddOptions<PaperApiOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(ApiKeyIsPresent, "ApiKey is required")
            .Validate(BaseUrlIsValid, "BaseUrl must be a valid absolute URI")
            .ValidateOnStart();

        services.AddHttpClient<IPaperApiClient, PaperApiClient>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<PaperApiOptions>>().Value;
            client.BaseAddress = opts.ResolveBaseUri();
        });

        return services;
    }

    /// <summary>
    /// Registers <see cref="IPaperApiClient"/> and configures <see cref="PaperApiOptions"/> via delegate.
    /// </summary>
    public static IServiceCollection AddPaperApiClient(this IServiceCollection services, Action<PaperApiOptions> configure)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configure is null) throw new ArgumentNullException(nameof(configure));

        services.AddOptions<PaperApiOptions>()
            .Configure(configure)
            .Validate(ApiKeyIsPresent, "ApiKey is required")
            .Validate(BaseUrlIsValid, "BaseUrl must be a valid absolute URI")
            .ValidateOnStart();

        services.AddHttpClient<IPaperApiClient, PaperApiClient>((sp, client) =>
        {
            var opts = sp.GetRequiredService<IOptions<PaperApiOptions>>().Value;
            client.BaseAddress = opts.ResolveBaseUri();
        });

        return services;
    }

    private static bool ApiKeyIsPresent(PaperApiOptions options) => !string.IsNullOrWhiteSpace(options.ApiKey);

    private static bool BaseUrlIsValid(PaperApiOptions options) => options.IsBaseUrlValid();
}