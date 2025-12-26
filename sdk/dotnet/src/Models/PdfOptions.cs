using System.Text.Json.Serialization;

namespace PaperApi.Models;

/// <summary>
/// Optional wkhtmltopdf tweaks applied when rendering the document.
/// </summary>
public sealed class PdfOptions
{
    [JsonPropertyName("pageSize")]
    public string? PageSize { get; set; }

    [JsonPropertyName("orientation")]
    public string? Orientation { get; set; }

    [JsonPropertyName("marginTop")]
    public decimal? MarginTop { get; set; }

    [JsonPropertyName("marginRight")]
    public decimal? MarginRight { get; set; }

    [JsonPropertyName("marginBottom")]
    public decimal? MarginBottom { get; set; }

    [JsonPropertyName("marginLeft")]
    public decimal? MarginLeft { get; set; }

    [JsonPropertyName("printMediaType")]
    public bool? PrintMediaType { get; set; }

    [JsonPropertyName("disableSmartShrinking")]
    public bool? DisableSmartShrinking { get; set; }

    [JsonPropertyName("enableJavascript")]
    public bool? EnableJavascript { get; set; }

    [JsonPropertyName("disableJavascript")]
    public bool? DisableJavascript { get; set; }

    [JsonPropertyName("headerLeft")]
    public string? HeaderLeft { get; set; }

    [JsonPropertyName("headerCenter")]
    public string? HeaderCenter { get; set; }

    [JsonPropertyName("headerRight")]
    public string? HeaderRight { get; set; }

    [JsonPropertyName("footerLeft")]
    public string? FooterLeft { get; set; }

    [JsonPropertyName("footerCenter")]
    public string? FooterCenter { get; set; }

    [JsonPropertyName("footerRight")]
    public string? FooterRight { get; set; }

    [JsonPropertyName("headerSpacing")]
    public decimal? HeaderSpacing { get; set; }

    [JsonPropertyName("footerSpacing")]
    public decimal? FooterSpacing { get; set; }

    [JsonPropertyName("headerHtml")]
    public string? HeaderHtml { get; set; }

    [JsonPropertyName("footerHtml")]
    public string? FooterHtml { get; set; }

    [JsonPropertyName("dpi")]
    public int? Dpi { get; set; }

    [JsonPropertyName("zoom")]
    public double? Zoom { get; set; }

    [JsonPropertyName("imageDpi")]
    public int? ImageDpi { get; set; }

    [JsonPropertyName("imageQuality")]
    public int? ImageQuality { get; set; }

    [JsonPropertyName("lowQuality")]
    public bool? LowQuality { get; set; }

    [JsonPropertyName("images")]
    public bool? Images { get; set; }

    [JsonPropertyName("noImages")]
    public bool? NoImages { get; set; }
}
