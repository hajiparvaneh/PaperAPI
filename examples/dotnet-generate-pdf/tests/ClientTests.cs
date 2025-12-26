using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PaperApi;
using PaperApi.Models;
using Xunit;

namespace PaperApiExample.Tests;

public class ClientTests
{
    [Fact]
    public async Task GeneratePdfAsync_SendsBearerAuth_AndReturnsBytes()
    {
        var expectedBytes = new byte[] { 1, 2, 3 };
        var handler = new RecordingHandler(expectedBytes);
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com/")
        };

        var client = new PaperApiClient(httpClient, new PaperApiOptions
        {
            ApiKey = "test-key",
            BaseUrl = "https://example.com/"
        });

        var payload = new PdfGenerateRequest { Html = "<p>Hi</p>" };

        var result = await client.GeneratePdfAsync(payload);

        Assert.Equal(expectedBytes, result);
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("Bearer", handler.LastRequest.Headers.Authorization?.Scheme);
        Assert.Equal("test-key", handler.LastRequest.Headers.Authorization?.Parameter);
        Assert.Equal("https://example.com/v1/generate", handler.LastRequest.RequestUri!.ToString());
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly byte[] _responseBytes;

        public RecordingHandler(byte[] responseBytes)
        {
            _responseBytes = responseBytes;
        }

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var content = new ByteArrayContent(_responseBytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };
            return Task.FromResult(response);
        }
    }
}
