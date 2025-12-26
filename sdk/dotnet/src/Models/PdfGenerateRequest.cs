using System.Text.Json.Serialization;

namespace PaperApi.Models;

/// <summary>
/// Represents the JSON body sent to the PaperAPI PDF generation endpoint.
/// </summary>
public sealed class PdfGenerateRequest
{
    [JsonPropertyName("html")]
    public string Html { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public PdfOptions? Options { get; set; }
        = null;
}
