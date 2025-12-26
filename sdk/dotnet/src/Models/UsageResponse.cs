using System;
using System.Text.Json.Serialization;

namespace PaperApi.Models;

/// <summary>
/// Represents the per-plan usage counters returned by PaperAPI.
/// </summary>
public sealed class UsageResponse
{
    [JsonPropertyName("used")]
    public int Used { get; set; }

    [JsonPropertyName("monthlyLimit")]
    public int MonthlyLimit { get; set; }

    [JsonPropertyName("remaining")]
    public int Remaining { get; set; }

    [JsonPropertyName("overage")]
    public int Overage { get; set; }

    [JsonPropertyName("nextRechargeAt")]
    public DateTimeOffset NextRechargeAt { get; set; }
}
