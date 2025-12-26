using PaperApi;
using PaperApi.Models;

LoadEnvFromRoot();

var apiKey = GetRequiredEnv("PAPERAPI_API_KEY");
var baseUrl = GetRequiredEnv("PAPERAPI_BASE_URL");

var client = new PaperApiClient(new PaperApiOptions
{
    ApiKey = apiKey,
    BaseUrl = baseUrl
});

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

var pdfBytes = await client.GeneratePdfAsync(new PdfGenerateRequest
{
    Html = html,
    Options = new PdfOptions
    {
        PageSize = "A4",
        MarginTop = 5,
        MarginBottom = 5
    }
});

var outputPath = Path.Combine(
    Directory.GetCurrentDirectory(),
    $"paperapi-invoice-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}.pdf");
await File.WriteAllBytesAsync(outputPath, pdfBytes);

Console.WriteLine($"Saved PDF to {outputPath}");

static void LoadEnvFromRoot()
{
    var envFile = FindNearestEnvFile();
    if (envFile is null)
    {
        Console.WriteLine("No .env file found. Falling back to existing environment variables.");
        return;
    }

    Console.WriteLine($"Loading configuration from {envFile}");
    foreach (var rawLine in File.ReadAllLines(envFile))
    {
        var line = rawLine.Trim();
        if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
        {
            continue;
        }

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim();
        Environment.SetEnvironmentVariable(key, value);
    }
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
