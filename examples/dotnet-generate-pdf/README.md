# .NET example – Generate a PDF

This console sample references the local `.NET` SDK and demonstrates registering the SDK via DI, posting raw HTML to PaperAPI, and saving the resulting PDF to disk.

## Prerequisites
1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download).
2. Update the repository `.env` file with both `PAPERAPI_BASE_URL` and `PAPERAPI_API_KEY` (sign up for free at [paperapi.de](https://paperapi.de)).

## Install the SDK
If you're recreating this sample outside of the repository, pull the NuGet package into your project and add the DI primitives:

```bash
dotnet add package PaperApi
dotnet add package Microsoft.Extensions.DependencyInjection
```

## Run the example
```bash
cd examples/dotnet-generate-pdf
 dotnet run
```

The app automatically looks for an `.env` file in the repository root and populates the `PAPERAPI_BASE_URL`/`PAPERAPI_API_KEY` environment variables on startup. You can override them manually if you prefer.

The program wires the SDK using `AddPaperApiClient` and resolves `IPaperApiClient` from the service provider, matching the recommended pattern for ASP.NET Core and worker services.

## Run the tests
```bash
cd examples/dotnet-generate-pdf/tests
dotnet test
```
The unit test stubs HTTP responses with a custom handler to verify the client sends the expected authorization header and returns the mocked PDF bytes—no network calls are made.

## Expected output
```
Loading configuration from /.../.env
Checking PaperAPI health...
Health endpoint responded successfully.
Fetching authenticated account info...
Logged in as ...
Usage this period: ...
Generating a PDF synchronously...
Saved synchronous PDF to /path/to/repo/examples/dotnet-generate-pdf/paperapi-sync-invoice-20240601010101.pdf
Submitting an asynchronous PDF job...
Poll 1: job status is 'succeeded'.
Asynchronous job completed. Saved PDF to /path/to/repo/examples/dotnet-generate-pdf/paperapi-async-invoice-20240601010101.pdf
```

Adjust the HTML snippet and `PdfOptions` in `Program.cs` to match your real template and margins.

> Want to process jobs asynchronously? Replace `GeneratePdfAsync` with `EnqueuePdfJobAsync`, poll `GetJobStatusAsync`, and download the result when it succeeds.
