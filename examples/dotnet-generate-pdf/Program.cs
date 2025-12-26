using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PaperApi;
using PaperApi.Models;

LoadEnvFromRoot();

var apiKey = GetRequiredEnv("PAPERAPI_API_KEY");
var baseUrl = GetRequiredEnv("PAPERAPI_BASE_URL");

var services = new ServiceCollection();
services.AddPaperApiClient(opts =>
{
  opts.ApiKey = apiKey;
  opts.BaseUrl = baseUrl;
});

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<IPaperApiClient>();

var html = """
<!doctype html>
<html>
  <head>
    <meta charset="utf-8" />
    <style>
      body { font-family: "Helvetica Neue", Arial, sans-serif; padding: 24px; }
      h1 { margin-bottom: 4px; }
      table { width: 100%; border-collapse: collapse; margin-top: 16px; }
      th, td { padding: 8px; border-bottom: 1px solid #ececec; text-align: left; }
      tfoot td { font-weight: bold; }
    </style>
  </head>
  <body>
    <h1>Invoice #INV-2042</h1>
    <p>Issued to Sloan Ada on 2024-06-01</p>
    <table>
      <thead>
        <tr>
          <th>Item</th>
          <th>Qty</th>
          <th>Price</th>
        </tr>
      </thead>
      <tbody>
        <tr>
          <td>Premium Support</td>
          <td>1</td>
          <td>€100.00</td>
        </tr>
        <tr>
          <td>PaperAPI Usage</td>
          <td>1,000 PDFs</td>
          <td>€49.00</td>
        </tr>
      </tbody>
      <tfoot>
        <tr>
          <td colspan="2">Total</td>
          <td>€149.00</td>
        </tr>
      </tfoot>
    </table>
  </body>
</html>
""";

Console.WriteLine("Checking PaperAPI health...");
await EnsureHealthAsync(client);
await DescribeAccountAsync(client);
await GeneratePdfSynchronouslyAsync(client, html);
await RunAsyncJobWorkflowAsync(client, html);
Console.WriteLine("All API calls completed successfully.");

static async Task EnsureHealthAsync(IPaperApiClient client)
{
  await client.CheckHealthAsync();
  Console.WriteLine("Health endpoint responded successfully.\n");
}

static async Task DescribeAccountAsync(IPaperApiClient client)
{
  Console.WriteLine("Fetching authenticated account info...");
  var profile = await client.GetWhoAmIAsync();
  Console.WriteLine($"Logged in as {profile.Name} <{profile.Email}> on the {profile.Plan.Name} plan.");

  var usage = await client.GetUsageSummaryAsync();
  Console.WriteLine($"Usage this period: {usage.Used}/{usage.MonthlyLimit} PDFs (remaining: {usage.Remaining}, overage: {usage.Overage}). Next reset on {usage.NextRechargeAt:yyyy-MM-dd}.\n");
}

static async Task GeneratePdfSynchronouslyAsync(IPaperApiClient client, string html)
{
  Console.WriteLine("Generating a PDF synchronously...");
  var pdfBytes = await client.GeneratePdfAsync(BuildInvoiceRequest(html));
  var path = await WritePdfToDiskAsync(pdfBytes, "sync");
  Console.WriteLine($"Saved synchronous PDF to {path}\n");
}

static async Task RunAsyncJobWorkflowAsync(IPaperApiClient client, string html)
{
  Console.WriteLine("Submitting an asynchronous PDF job...");
  var job = await client.EnqueuePdfJobAsync(BuildInvoiceRequest(html));
  Console.WriteLine($"Job {job.Id} queued with status '{job.Status}'. Polling for completion...");

  const int maxAttempts = 15;
  var delay = TimeSpan.FromSeconds(2);

  for (var attempt = 1; attempt <= maxAttempts; attempt++)
  {
    var status = await client.GetJobStatusAsync(job.Id);
    Console.WriteLine($"Poll {attempt}: job status is '{status.Status}'.");

    if (IsStatus(status.Status, "succeeded"))
    {
      var pdfBytes = await client.DownloadJobResultAsync(job.Id);
      var path = await WritePdfToDiskAsync(pdfBytes, "async");
      Console.WriteLine($"Asynchronous job completed. Saved PDF to {path}\n");
      return;
    }

    if (IsStatus(status.Status, "failed"))
    {
      throw new InvalidOperationException($"Job {job.Id} failed: {status.ErrorMessage ?? "Unknown error"}");
    }

    await Task.Delay(delay);
  }

  throw new TimeoutException($"Job {job.Id} did not complete within the polling window. Try increasing maxAttempts.");
}

static PdfGenerateRequest BuildInvoiceRequest(string html) => new()
{
  Html = html,
  Options = new PdfOptions
  {
    PageSize = "A4",
    MarginTop = 5,
    MarginBottom = 5
  }
};

static async Task<string> WritePdfToDiskAsync(byte[] pdfBytes, string prefix)
{
  var fileName = $"paperapi-{prefix}-invoice-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.pdf";
  var outputPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
  await File.WriteAllBytesAsync(outputPath, pdfBytes);
  return outputPath;
}

static bool IsStatus(string status, string expected) =>
    string.Equals(status, expected, StringComparison.OrdinalIgnoreCase);

static void LoadEnvFromRoot()
{
  var envFile = FindNearestEnvFile();
  if (envFile is null)
  {
    Console.WriteLine("No .env file found. Falling back to existing environment variables.");
    return;
  }

  Console.WriteLine($"Loading configuration from {envFile}");
  File.ReadAllLines(envFile)
    .Select(rawLine => rawLine.Trim())
    .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith('#'))
    .Select(line => (line, separatorIndex: line.IndexOf('=')))
    .Where(x => x.separatorIndex > 0)
    .Select(x => (key: x.line[..x.separatorIndex].Trim(), value: x.line[(x.separatorIndex + 1)..].Trim()))
    .ToList()
    .ForEach(x => Environment.SetEnvironmentVariable(x.key, x.value));
}

static string? FindNearestEnvFile()
{
  var current = Directory.GetCurrentDirectory();
  while (!string.IsNullOrEmpty(current))
  {
    var candidate = Path.Combine(current, ".env");
    if (File.Exists(candidate))
    {
      return candidate;
    }

    var parent = Directory.GetParent(current);
    if (parent is null)
    {
      break;
    }
    current = parent.FullName;
  }

  return null;
}

static string GetRequiredEnv(string key)
{
  var value = Environment.GetEnvironmentVariable(key);
  if (string.IsNullOrWhiteSpace(value))
  {
    throw new InvalidOperationException($"Environment variable '{key}' was not found. Did you update the .env file?");
  }

  return value;
}
