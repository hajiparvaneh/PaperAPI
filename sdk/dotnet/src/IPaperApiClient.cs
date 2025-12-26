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
    Task<byte[]> GeneratePdfAsync(PdfGenerateRequest request, CancellationToken cancellationToken = default);

    Task<PdfJobStatusResponse> EnqueuePdfJobAsync(PdfGenerateRequest request, CancellationToken cancellationToken = default);

    Task<PdfJobStatusResponse> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default);

    Task<byte[]> DownloadJobResultAsync(Guid jobId, CancellationToken cancellationToken = default);

    Task<UsageResponse> GetUsageSummaryAsync(CancellationToken cancellationToken = default);

    Task<WhoAmIResponse> GetWhoAmIAsync(CancellationToken cancellationToken = default);

    Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default);
}