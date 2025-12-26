# PaperAPI SDKs and Examples

Welcome to the public PaperAPI SDK playground. This repository contains language-specific SDKs, framework-agnostic examples, and CI/CD automation that help you start building with [PaperAPI](https://paperapi.de) immediately.

## Get your API key
1. Create a free PaperAPI account at [paperapi.de](https://paperapi.de).
2. Head to your dashboard and copy the generated API key.
3. Paste the key into the provided `.env` file alongside the default API base URL `https://api.paperapi.de`.

```
PAPERAPI_BASE_URL=https://api.paperapi.de
PAPERAPI_API_KEY=REPLACE_WITH_YOUR_API_KEY
```

Never commit production credentials. The placeholder `.env` file exists so the examples can be run without extra setup.

## Repository layout

```
Pubic_repo_sdk_example/
├── README.md
├── LICENSE
├── .env
├── sdk/
│   ├── dotnet/          # First-class SDK, NuGet-ready
│   ├── js/              # Placeholder for the upcoming npm package
│   └── python/          # Placeholder for the upcoming PyPI package
├── examples/
│   ├── dotnet-generate-pdf/
│   ├── js-generate-pdf/
│   └── python-generate-pdf/
├── docs/
└── .github/workflows/
```

Each language folder is self-contained with its own README and publishing strategy. .NET is the first fully implemented SDK; the JavaScript and Python directories contain documentation stubs until their implementations are finalized.

## Quick start: .NET SDK

* Install the [.NET 8 SDK](https://dotnet.microsoft.com/download).
* Navigate to `sdk/dotnet` and restore packages: `dotnet restore`.
* Run the sample generator: `cd ../../examples/dotnet-generate-pdf && dotnet run`.

The example references the local SDK project so you can iterate on the client library while keeping the example pinned to the current commit.

The `PaperApiClient` included here mirrors every public endpoint:

- `GeneratePdfAsync` for synchronous PDF bytes.
- `EnqueuePdfJobAsync` / `GetJobStatusAsync` / `DownloadJobResultAsync` for queued jobs.
- `GetUsageSummaryAsync`, `GetWhoAmIAsync`, and `CheckHealthAsync` for account telemetry.

## Documentation
Deep-dive docs live under `docs/`. They describe authentication, request/response formats, and release guidelines, keeping the root README intentionally concise.

## Contributing
1. Fork the repository and create a feature branch.
2. Make your changes (linting/formatting enforced by CI).
3. Submit a pull request against `main`.

CI pipelines automatically build, test, and pack SDKs. Publishing to NuGet/npm/PyPI happens only from trusted release workflows.
