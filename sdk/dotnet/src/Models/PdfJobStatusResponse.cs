using System;
using System.Text.Json.Serialization;

namespace PaperApi.Models;

/// <summary>
/// Represents the job envelope returned by asynchronous endpoints.
/// </summary>
public sealed class PdfJobStatusResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("downloadUrl")]
    public string? DownloadUrl { get; set; }

    [JsonPropertyName("jobStatusUrl")]
    public string? JobStatusUrl { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("expiresAt")]
    public DateTimeOffset ExpiresAt { get; set; }

    [JsonPropertyName("links")]
    public PdfJobLinks Links { get; set; } = new();
}

public sealed class PdfJobLinks
{
    [JsonPropertyName("self")]
    public string Self { get; set; } = string.Empty;

    [JsonPropertyName("result")]
    public string? Result { get; set; }
}
