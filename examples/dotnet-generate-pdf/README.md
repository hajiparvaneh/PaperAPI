# .NET example – Generate a PDF

This console sample references the local `.NET` SDK and demonstrates how to post raw HTML to PaperAPI and save the resulting PDF to disk.

## Prerequisites
1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download).
2. Update the repository `.env` file with both `PAPERAPI_BASE_URL` and `PAPERAPI_API_KEY` (sign up for free at [paperapi.de](https://paperapi.de)).

## Install the SDK
If you're recreating this sample outside of the repository, pull the NuGet package into your project:

```bash
dotnet add package PaperApi
```

## Run the example
```bash
cd examples/dotnet-generate-pdf
 dotnet run
```

The app automatically looks for an `.env` file in the repository root and populates the `PAPERAPI_BASE_URL`/`PAPERAPI_API_KEY` environment variables on startup. You can override them manually if you prefer.

## Expected output
```
Loading configuration from /.../.env
Saved PDF to /path/to/repo/examples/dotnet-generate-pdf/paperapi-invoice-20240601010101.pdf
```

Adjust the HTML snippet and `PdfOptions` in `Program.cs` to match your real template and margins.

> Want to process jobs asynchronously? Replace `GeneratePdfAsync` with `EnqueuePdfJobAsync`, poll `GetJobStatusAsync`, and download the result when it succeeds.
