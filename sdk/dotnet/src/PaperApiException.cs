using System;
using System.Net;
using System.Text.Json;

namespace PaperApi;

/// <summary>
/// Represents API failures with contextual information.
/// </summary>
public sealed class PaperApiException : Exception
{
    public PaperApiException(HttpStatusCode statusCode, string? errorCode, string? message, string? responseBody)
        : base(message ?? $"PaperAPI request failed with status code {(int)statusCode} ({statusCode})")
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ResponseBody = responseBody;
    }

    public HttpStatusCode StatusCode { get; }

    public string? ErrorCode { get; }

    public string? ResponseBody { get; }

    public static PaperApiException FromJson(HttpStatusCode statusCode, string payload)
    {
        try
        {
            var error = JsonSerializer.Deserialize<PaperApiErrorResponse>(payload);
            return new PaperApiException(statusCode, error?.ErrorCode, error?.Message ?? error?.Error, payload);
        }
        catch (JsonException)
        {
            return new PaperApiException(statusCode, null, null, payload);
        }
    }

    private sealed class PaperApiErrorResponse
    {
        public string? Error { get; set; }

        public string? ErrorCode { get; set; }

        public string? Message { get; set; }
    }
}
