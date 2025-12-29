# PaperAPI JavaScript example

This example shows how to consume the `@paperapi/sdk` npm package from a TypeScript script. It loads the API key from the repository `.env` file, hits every public endpoint (health check, usage, profile, sync/async generation), and writes the generated PDF to disk.

## Prerequisites
- Node.js 18 or newer.
- A valid PaperAPI key stored in the root `.env` file:

```
PAPERAPI_BASE_URL=https://api.paperapi.de
PAPERAPI_API_KEY=REPLACE_WITH_YOUR_API_KEY
```

## Running the script

```bash
cd examples/js-generate-pdf
npm install
npm run dev
```

The script logs the authenticated user, current usage limits, and the asynchronous job status. It saves a rendered PDF to `./output/invoice.pdf`.

### Developing against the local SDK

Before the npm package is published you can wire the example to the local SDK source:

```bash
# Build the SDK and link it locally
cd ../../sdk/js
npm install
npm run build
npm pack --pack-destination ../..

# Install from the generated tarball (update the filename)
cd ../examples/js-generate-pdf
npm install ../../paperapi-sdk-0.1.0.tgz
```

After the package is published you can remove the tarball install – the declared `@paperapi/sdk` dependency will resolve straight from npm.
