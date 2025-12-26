using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using PaperApi.Models;

namespace PaperApi;

/// <summary>
/// Lightweight HTTP client for PaperAPI.
/// </summary>
public sealed class PaperApiClient : IDisposable
{
    private static readonly ProductInfoHeaderValue UserAgentHeader = new("PaperApiDotnetSdk", "0.1.0");

    private readonly HttpClient _httpClient;
    private readonly bool _ownsClient;
    private readonly PaperApiOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public PaperApiClient(PaperApiOptions options, HttpClient? httpClient = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.EnsureValid();

        if (httpClient is null)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = _options.ResolveBaseUri()
            };
            _ownsClient = true;
        }
        else
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress ??= _options.ResolveBaseUri();
        }

        var headers = _httpClient.DefaultRequestHeaders;
        if (!headers.UserAgent.Contains(UserAgentHeader))
        {
            headers.UserAgent.Add(UserAgentHeader);
        }
    }

    /// <summary>
    /// Generates a PDF synchronously and returns the raw document bytes.
    /// </summary>
    public async Task<byte[]> GeneratePdfAsync(PdfGenerateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateGenerateRequest(request);

        using var httpRequest = CreateRequest(HttpMethod.Post, "v1/generate", "application/pdf");
        httpRequest.Content = SerializeJson(request);
        return await SendForBytesAsync(httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Enqueues a background job that renders the PDF asynchronously.
    /// </summary>
    public async Task<PdfJobStatusResponse> EnqueuePdfJobAsync(PdfGenerateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateGenerateRequest(request);

        using var httpRequest = CreateRequest(HttpMethod.Post, "v1/generate-async", "application/json");
        httpRequest.Content = SerializeJson(request);
        return await SendForJsonAsync<PdfJobStatusResponse>(httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the status of an existing PDF job.
    /// </summary>
    public async Task<PdfJobStatusResponse> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Get, $"jobs/{jobId}", "application/json");
        return await SendForJsonAsync<PdfJobStatusResponse>(httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Downloads the PDF produced by an asynchronous job.
    /// </summary>
    public async Task<byte[]> DownloadJobResultAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Get, $"jobs/{jobId}/result", "application/pdf");
        return await SendForBytesAsync(httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the usage summary for the authenticated account.
    /// </summary>
    public async Task<UsageResponse> GetUsageSummaryAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Get, "v1/usage", "application/json");
        return await SendForJsonAsync<UsageResponse>(httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns the user profile tied to the API key.
    /// </summary>
    public async Task<WhoAmIResponse> GetWhoAmIAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Get, "v1/whoami", "application/json");
        return await SendForJsonAsync<WhoAmIResponse>(httpRequest, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Calls the /health endpoint and throws if the service is unavailable.
    /// </summary>
    public async Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        using var httpRequest = CreateRequest(HttpMethod.Get, "health", "application/json");
        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        return true;
    }

    public void Dispose()
    {
        if (_ownsClient)
        {
            _httpClient.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    private static void ValidateGenerateRequest(PdfGenerateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Html))
        {
            throw new ArgumentException("Html is required.", nameof(request));
        }
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string relativeUrl, string accept)
    {
        var request = new HttpRequestMessage(method, relativeUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        if (!string.IsNullOrWhiteSpace(accept))
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
        }
        return request;
    }

    private StringContent SerializeJson(object payload)
    {
        var content = JsonSerializer.Serialize(payload, _serializerOptions);
        return new StringContent(content, Encoding.UTF8, "application/json");
    }

    private async Task<T> SendForJsonAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var result = JsonSerializer.Deserialize<T>(json, _serializerOptions);
        if (result is null)
        {
            throw new PaperApiException(response.StatusCode, null, $"PaperAPI returned an empty response for {typeof(T).Name}.", json);
        }

        return result;
    }

    private async Task<byte[]> SendForBytesAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response).ConfigureAwait(false);
        return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorBody = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        throw PaperApiException.FromJson(response.StatusCode, errorBody);
    }
}
