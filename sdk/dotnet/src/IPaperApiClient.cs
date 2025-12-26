using System;
using System.Threading;
using System.Threading.Tasks;
using PaperApi.Models;

namespace PaperApi;

/// <summary>
/// Abstraction for the PaperAPI client to enable DI and testing.
/// </summary>
public interface IPaperApiClient
{
    /// <summary>
    /// Generates a PDF synchronously and returns the raw document bytes.
    /// </summary>
    Task<byte[]> GeneratePdfAsync(PdfGenerateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enqueues a background job that renders the PDF asynchronously.
    /// </summary>
    Task<PdfJobStatusResponse> EnqueuePdfJobAsync(PdfGenerateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the status of an existing PDF job.
    /// </summary>
    Task<PdfJobStatusResponse> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the PDF produced by an asynchronous job.
    /// </summary>
    Task<byte[]> DownloadJobResultAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the usage summary for the authenticated account.
    /// </summary>
    Task<UsageResponse> GetUsageSummaryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the user profile tied to the API key.
    /// </summary>
    Task<WhoAmIResponse> GetWhoAmIAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls the /health endpoint and throws if the service is unavailable.
    /// </summary>
    Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default);
}