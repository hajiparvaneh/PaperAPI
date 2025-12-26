using System;
using System.Text.Json.Serialization;

namespace PaperApi.Models;

/// <summary>
/// Represents the authenticated user profile associated with the API key.
/// </summary>
public sealed class WhoAmIResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("plan")]
    public WhoAmIPlanResponse Plan { get; set; } = new();
}

public sealed class WhoAmIPlanResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("interval")]
    public string Interval { get; set; } = "monthly";

    [JsonPropertyName("monthlyLimit")]
    public int MonthlyLimit { get; set; }

    [JsonPropertyName("priceCents")]
    public int PriceCents { get; set; }
}
