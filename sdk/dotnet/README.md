# PaperAPI .NET SDK

The PaperAPI .NET SDK is a lightweight, dependency-free HTTP client that wraps the official PaperAPI endpoints and is ready to be published to NuGet.

## Account, API key, and pricing
- Create a free account and grab your sandbox API key at https://paperapi.de/ (no card required).
- Sandbox includes 50 PDFs/month and 5 requests per minute for prototyping; no overages.
- Paid plans increase quotas and performance: Starter (1k PDFs/month), Pro (5k PDFs/month), Business (20k+ PDFs/month with burst traffic). See full details and overage rates at https://paperapi.de/pricing.

## Requirements
- .NET 8.0 SDK (the library targets `netstandard2.0` for broad compatibility).

## Development workflow
```bash
# Restore dependencies
cd sdk/dotnet
 dotnet restore

# Build and run analyzers
 dotnet build --configuration Release

# Create a NuGet package
 dotnet pack --configuration Release --include-symbols --output ./artifacts
```

The generated `.nupkg` file includes XML documentation and the README file for rich NuGet UI rendering.

## Usage
```csharp
using System.IO;
using PaperApi;
using PaperApi.Models;

var client = new PaperApiClient(new PaperApiOptions
{
    ApiKey = Environment.GetEnvironmentVariable("PAPERAPI_API_KEY")!,
    BaseUrl = Environment.GetEnvironmentVariable("PAPERAPI_BASE_URL")
});

var pdfBytes = await client.GeneratePdfAsync(new PdfGenerateRequest
{
    Html = "<html><body><h1>Hello from PaperAPI</h1></body></html>",
    Options = new PdfOptions
    {
        PageSize = "A4",
        MarginTop = 5,
        MarginBottom = 5
    }
});

await File.WriteAllBytesAsync("invoice.pdf", pdfBytes);
Console.WriteLine("PDF written to invoice.pdf");
```

> Tip: keep both `PAPERAPI_BASE_URL` and `PAPERAPI_API_KEY` in a `.env` file (as shown in the root README) so every environment resolves the correct API endpoint without hardcoding it.

### ASP.NET Core DI
```csharp
// appsettings.json
// "PaperApi": { "ApiKey": "...", "BaseUrl": "https://api.paperapi.de/" }

builder.Services.AddPaperApiClient(builder.Configuration);

var app = builder.Build();

app.MapGet("/pdf", async (IPaperApiClient client) =>
{
    var bytes = await client.GeneratePdfAsync(new PdfGenerateRequest { Html = "<h1>Hello</h1>" });
    return Results.File(bytes, "application/pdf", "hello.pdf");
});
```

### Testing
- Use `IPaperApiClient` to mock the SDK in unit tests.
- For integration-style tests, construct `PaperApiClient` with a custom `HttpMessageHandler` to return canned responses without hitting the network.

### Supported endpoints

`PaperApiClient` wraps every public API route:

| Method | SDK call | Description |
| --- | --- | --- |
| `POST /v1/generate` | `GeneratePdfAsync` | Immediate PDF generation (returns bytes). |
| `POST /v1/generate-async` | `EnqueuePdfJobAsync` | Queue a job and receive a job envelope. |
| `GET /jobs/{id}` | `GetJobStatusAsync` | Poll job state (queued, running, succeeded, failed). |
| `GET /jobs/{id}/result` | `DownloadJobResultAsync` | Download the completed PDF. |
| `GET /v1/usage` | `GetUsageSummaryAsync` | Inspect current-plan usage counters. |
| `GET /v1/whoami` | `GetWhoAmIAsync` | Retrieve the authenticated account + plan details. |
| `GET /health` | `CheckHealthAsync` | Sanity check the service availability. |

Every method throws `PaperApiException` when the API returns a non-success status, exposing the HTTP status code, optional error code, and response body so you can log or retry intelligently.

## Publishing
Publishing is handled by GitHub Actions (`.github/workflows/sdk-ci.yml`). The workflow:
1. Restores, builds, and packs the SDK for every PR and push to `main`.
2. When `main` is updated, the second job pushes the `.nupkg` to NuGet if the `NUGET_API_KEY` secret exists.
3. You can also run the workflow manually via `workflow_dispatch` to validate artifacts before tagging a release.

## Versioning
Increment the `Version` property in `PaperApi.csproj` following semantic versioning. CI automatically fails if you forget to update the version when the API surface changes.
