import 'dotenv/config';
import fs from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { PaperApiClient, PaperApiError } from '@paperapi/sdk';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const OUTPUT_DIR = path.join(__dirname, 'output');

const requireEnv = (key: string): string => {
  const value = process.env[key];
  if (!value) {
    throw new Error(`Missing environment variable ${key}. Did you populate .env?`);
  }
  return value;
};

async function main(): Promise<void> {
  const apiKey = requireEnv('PAPERAPI_API_KEY');
  const baseUrl = process.env.PAPERAPI_BASE_URL;

  const client = new PaperApiClient({
    apiKey,
    baseUrl
  });

  await client.checkHealth();
  console.log('✅ PaperAPI health check succeeded');

  const profile = await client.getWhoAmI();
  console.log(`Authenticated as ${profile.name} (${profile.email}) on plan ${profile.plan.name}`);

  const usage = await client.getUsageSummary();
  console.log(
    `Usage: ${usage.used}/${usage.monthlyLimit} (resets ${usage.nextRechargeAt.toISOString()})`
  );

  const summaryHtml = `<html>
    <body style="font-family: sans-serif;">
      <h1 style="color:#0f172a;">PaperAPI JavaScript SDK</h1>
      <p>Generated at ${new Date().toISOString()}</p>
      <ul>
        <li>Account: ${profile.email}</li>
        <li>Plan: ${profile.plan.name}</li>
        <li>Usage: ${usage.used}/${usage.monthlyLimit}</li>
      </ul>
    </body>
  </html>`;

  const pdfBytes = await client.generatePdf({
    html: summaryHtml,
    options: {
      pageSize: 'A4',
      marginTop: 10,
      marginBottom: 10
    }
  });

  await fs.mkdir(OUTPUT_DIR, { recursive: true });
  const outputPath = path.join(OUTPUT_DIR, 'invoice.pdf');
  await fs.writeFile(outputPath, Buffer.from(pdfBytes));
  console.log(`📄 PDF saved to ${outputPath}`);

  const asyncJob = await client.enqueuePdfJob({
    html: '<html><body><h1>Queued job</h1><p>This renders asynchronously.</p></body></html>'
  });

  console.log(
    `📬 Enqueued job ${asyncJob.id} (${asyncJob.status}). Track at ${asyncJob.links.self}`
  );
}

main().catch((error) => {
  if (error instanceof PaperApiError) {
    console.error('PaperAPI error', {
      status: error.status,
      errorCode: error.errorCode,
      responseBody: error.responseBody
    });
  } else {
    console.error(error);
  }
  process.exit(1);
});
