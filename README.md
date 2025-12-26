![PaperAPI logo](paperapi-logo.png)

# PaperAPI SDKs and Examples

Welcome to the public PaperAPI SDK playground. This repository contains language-specific SDKs, framework-agnostic examples, and CI/CD automation that help you start building with [PaperAPI](https://paperapi.de) immediately.

## What is PaperAPI?

[PaperAPI](https://paperapi.de) is a hosted wkhtmltopdf platform that converts HTML into pixel-perfect PDFs without servers, fonts, or binary installs. It is built and hosted in the EU for GDPR-compliant workloads, and exposes both synchronous and asynchronous generation flows so you can either stream documents immediately or queue large jobs in the background.

- **Simple HTTP API**: Send HTML + options to `/v1/generate` or `/v1/generate-async` and receive bytes or a job envelope.
- **Operational safety**: Usage counters, audit logs, request IDs, retries, and structured errors make production monitoring straightforward.
- **EU residency**: All rendering happens on EU infrastructure with DPA, privacy, and security docs ready for procurement teams.
- **Batteries included**: Dashboards for API keys, usage, billing, logs, and download history, plus SDKs and examples so you can integrate in minutes.

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

## Examples

Every example has its own README with prerequisites and end-to-end instructions. Start with the implementation that matches your stack:

- [.NET – Generate a PDF](examples/dotnet-generate-pdf/README.md): console app that hits every PaperAPI endpoint (health, account metadata, sync generation, async jobs) and saves PDFs to disk.
- [JavaScript – Generate a PDF](examples/js-generate-pdf/README.md): placeholder until the TypeScript SDK is finalized.
- [Python – Generate a PDF](examples/python-generate-pdf/README.md): placeholder until the PyPI SDK ships.

## Documentation
Deep-dive docs live under `docs/`. They describe authentication, request/response formats, and release guidelines, keeping the root README intentionally concise.

## Contributing
1. Fork the repository and create a feature branch.
2. Make your changes (linting/formatting enforced by CI).
3. Submit a pull request against `main`.

CI pipelines automatically build, test, and pack SDKs. Publishing to NuGet/npm/PyPI happens only from trusted release workflows.
