using System;

namespace PaperApi;

/// <summary>
/// Configuration container for the <see cref="PaperApiClient"/>.
/// </summary>
public sealed class PaperApiOptions
{
    private const string DefaultBaseUrl = "https://api.paperapi.de/";

    /// <summary>
    /// PaperAPI API key. Required for every request.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Base URL of the PaperAPI service. Defaults to the public PaperAPI cloud endpoint.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    internal Uri ResolveBaseUri()
    {
        var value = string.IsNullOrWhiteSpace(BaseUrl) ? DefaultBaseUrl : BaseUrl;
        value = value.EndsWith("/", StringComparison.Ordinal) ? value : value + "/";
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            ? uri
            : throw new ArgumentException("BaseUrl must be a valid absolute URI", nameof(BaseUrl));
    }

    internal bool IsBaseUrlValid()
    {
        var value = string.IsNullOrWhiteSpace(BaseUrl) ? DefaultBaseUrl : BaseUrl;
        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }

    internal void EnsureValid()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new ArgumentException("ApiKey is required", nameof(ApiKey));
        }
        _ = ResolveBaseUri();
    }
}
