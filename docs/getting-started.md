# Getting Started with PaperAPI

## Overview
PaperAPI lets you generate high-fidelity PDF documents through a single HTTPS endpoint at `https://api.paperapi.de`. This document dives deeper into authentication, best practices, and the SDK release workflow.

## Authentication
1. Sign up at [paperapi.de](https://paperapi.de) and verify your email.
2. Copy your API key from the dashboard.
3. Store it securely. The samples rely on environment variables loaded from the repository `.env` file but production apps should use a vault (Azure Key Vault, AWS Secrets Manager, etc.).

Every HTTP request must include: `Authorization: Bearer <YOUR_API_KEY>`.

## Environment configuration
Always provide both the base URL and API key via environment variables:

```
PAPERAPI_BASE_URL=https://api.paperapi.de
PAPERAPI_API_KEY=REPLACE_WITH_YOUR_API_KEY
```

All examples load those variables from the root `.env` file before instantiating the SDK, which keeps the server endpoint configurable (e.g., for EU vs. self-hosted deployments) without recompiling.

## Request model
The PDF generation endpoints accept structured JSON. The current .NET SDK models the payload via `PdfGenerateRequest`:

```json
{
  "html": "<html><body><h1>Invoice #INV-4821</h1></body></html>",
  "options": {
    "pageSize": "A4",
    "marginTop": 5,
    "marginBottom": 5
  }
}
```

### Synchronous generation
`POST /v1/generate` renders the document immediately when invoked with an API key and responds with `application/pdf` bytes. Save the payload as a `.pdf` file to view it locally or upload it to your storage service.

### Asynchronous jobs
Authenticated users (API keys tied to accounts) can enqueue jobs via `POST /v1/generate-async`. The response is a job envelope:

```json
{
  "id": "45f2c2d9-2779-4d68-b7e0-65e4e44acf5d",
  "status": "Queued",
  "downloadUrl": null,
  "jobStatusUrl": "/jobs/45f2c2d9-2779-4d68-b7e0-65e4e44acf5d",
  "links": {
    "self": "/jobs/45f2c2d9-2779-4d68-b7e0-65e4e44acf5d",
    "result": null
  }
}
```

Poll `GET /jobs/{id}` until `status` becomes `Succeeded`, then download the PDF via `GET /jobs/{id}/result`.

### Account analytics
- `GET /v1/usage` returns how many PDFs you generated this month, the plan limit, remaining quota, and when the counter resets.
- `GET /v1/whoami` exposes the user profile (id, name, email) and the active subscription plan.
- `GET /health` returns `{ "status": "ok" }` so you can verify the API before starting workloads.

## Release workflow
1. **Feature branches**: Submit PRs targeting `main`. CI builds the SDK and runs analyzers.
2. **Versioning**: Update the `Version` property in `sdk/dotnet/PaperApi.csproj`. Semantic versioning is required.
3. **Packing**: `dotnet pack -c Release` emits a NuGet package including XML docs and the README.
4. **Publishing**: Use `dotnet nuget push bin/Release/PaperApi.<version>.nupkg -k <NUGET_API_KEY> -s https://api.nuget.org/v3/index.json` once the release workflow completes.

## Roadmap
- Ship JS SDK (TypeScript) with npm automation.
- Ship Python SDK targeting `pydantic` models.
- Provide more advanced recipe-style examples (batch jobs, template design).
