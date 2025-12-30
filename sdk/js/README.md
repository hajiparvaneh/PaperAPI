# PaperAPI JavaScript SDK

Official PaperAPI SDK for modern JavaScript runtimes (Node 18+, edge workers, and browsers). The package is published to npm as `@paperapi/sdk` and lets you render HTML to PDF (sync or async jobs) through PaperAPI's EU-hosted API.

## Account, API key, and pricing
- Create a free account and grab your sandbox API key at https://paperapi.de/ (no card required).
- Sandbox includes 50 PDFs/month and 5 requests per minute for prototyping; no overages.
- Paid plans increase quotas and performance: Starter (1k PDFs/month), Pro (5k PDFs/month), Business (20k+ PDFs/month with burst traffic). See full details and overage rates at https://paperapi.de/pricing.

## Features
- Tiny dependency footprint (`zod` for runtime validation).
- Works anywhere `fetch` exists; supply `options.fetch` for Node environments older than 18.
- Typed request/response models backed by runtime validation.
- Custom `PaperApiError` containing HTTP status code, error identifiers, and the raw response body for troubleshooting.
- Bundles ESM + CJS builds with TypeScript declarations.

## Installation

```bash
npm install @paperapi/sdk
# or
pnpm add @paperapi/sdk
```

## Quick start

```ts
import { PaperApiClient } from '@paperapi/sdk';
import fs from 'node:fs/promises';

const client = new PaperApiClient({
  apiKey: process.env.PAPERAPI_API_KEY!,
  baseUrl: process.env.PAPERAPI_BASE_URL // default: https://api.paperapi.de/
});

const pdf = await client.generatePdf({
  html: '<html><body><h1>PaperAPI ❤️ TypeScript</h1></body></html>',
  options: {
    pageSize: 'A4',
    marginTop: 5,
    marginBottom: 5
  }
});

await fs.writeFile('invoice.pdf', Buffer.from(pdf));
```

### Async jobs and metadata

```ts
const job = await client.enqueuePdfJob({ html: '<h1>Async job</h1>' });
console.log(job.status, job.links.self);

const refreshed = await client.getJobStatus(job.id);
if (refreshed.downloadUrl) {
  console.log(`Result ready at ${refreshed.downloadUrl}`);
}

const usage = await client.getUsageSummary();
console.log(`Used ${usage.used}/${usage.monthlyLimit} PDFs, resets ${usage.nextRechargeAt}`);

const profile = await client.getWhoAmI();
console.log(`Authenticated as ${profile.email} (${profile.plan.name})`);
```

### Error handling

All HTTP failures throw `PaperApiError`:

```ts
try {
  await client.generatePdf({ html: '' });
} catch (error) {
  if (error instanceof PaperApiError) {
    console.error(error.status, error.errorCode, error.responseBody);
  }
}
```

The error exposes the HTTP status, API error code (when provided), `x-request-id`, and the raw body so you can log or retry intelligently.

## Scripts

```bash
npm install        # restore dev dependencies
npm run typecheck  # strict TypeScript checks
npm run build      # emit dist/index.(mjs|cjs|d.ts)
```

`npm publish` runs `npm run build` automatically via the `prepare` script.

## Publishing workflow

1. Update the version in `package.json` following semver.
2. Run `npm install && npm run build` and verify `dist/`.
3. Login to npm (`npm login`) with an account that has publish rights.
4. From `sdk/js`, execute `npm publish --access public`.
5. Tag the release in Git (`git tag v0.x.y && git push origin v0.x.y`) so CI archives the artifacts.

GitHub Actions (`.github/workflows/sdk-ci.yml`) also builds the package and, when a tag prefixed with `v` is pushed, publishes the npm artifact using the `NPM_TOKEN` secret. See the repository README for CI details.
