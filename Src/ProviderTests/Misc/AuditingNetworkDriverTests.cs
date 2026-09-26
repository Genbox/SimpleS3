using System.Diagnostics.CodeAnalysis;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.ProviderTests.Misc;

public class AuditingNetworkDriverTests
{
    [Fact]
    public async Task CapturesBodiesWithoutConsumingResponseAndPreservesOwnership()
    {
        string directory = Path.Combine(Path.GetTempPath(), "SimpleS3-Audit-" + Guid.NewGuid().ToString("N"));
        byte[] payload = [0, 1, 128, 255];
        CancellationToken token = TestContext.Current.CancellationToken;
        await using NonSeekableStream source = new NonSeekableStream(payload);

        try
        {
            AuditingNetworkDriver driver = new AuditingNetworkDriver(new StubDriver(source), directory);
            await using MemoryStream upload = new MemoryStream(payload);
            HttpResponse response = await driver.SendRequestAsync<GetObjectResponse>(new GetObjectRequest("bucket", "key"), "https://example.test/bucket/%2E%2E?x=1", upload, token);

            await using (ContentStream content = response.Content!)
            {
                Assert.False(content.CanSeek);
                Assert.False(content.HasLength);
                Assert.Equal(0, source.BytesRead);
                await using MemoryStream destination = new MemoryStream();
                await content.CopyToAsync(destination, token);
                Assert.Equal(payload, destination.ToArray());
            }

            Assert.True(source.Disposed);
            string exchange = Assert.Single(Directory.GetDirectories(directory));
            Assert.Equal(payload, await File.ReadAllBytesAsync(Path.Combine(exchange, "request.body"), token));
            Assert.Equal(payload, await File.ReadAllBytesAsync(Path.Combine(exchange, "response.body"), token));
            Assert.Contains("https://example.test/bucket/%2E%2E?x=1", await File.ReadAllTextAsync(Path.Combine(exchange, "request.json"), token), StringComparison.Ordinal);
            Assert.Contains("503", await File.ReadAllTextAsync(Path.Combine(exchange, "response.json"), token), StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    [Fact]
    public async Task RecordsTransportFailureAndRethrowsOriginalException()
    {
        string directory = Path.Combine(Path.GetTempPath(), "SimpleS3-Audit-" + Guid.NewGuid().ToString("N"));

        try
        {
            AuditingNetworkDriver driver = new AuditingNetworkDriver(new FailingDriver(), directory);
            await Assert.ThrowsAsync<HttpRequestException>(() => driver.SendRequestAsync<GetObjectResponse>(new GetObjectRequest("bucket", "key"), "https://example.test", null, TestContext.Current.CancellationToken));
            string exchange = Assert.Single(Directory.GetDirectories(directory));
            Assert.Contains("transport failed", await File.ReadAllTextAsync(Path.Combine(exchange, "error.txt"), TestContext.Current.CancellationToken), StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    private sealed class StubDriver(Stream responseStream) : INetworkDriver
    {
        [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP004:Don't ignore created IDisposable", Justification = "Response content ownership is transferred to the caller.")]
        public async Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken token = default) where T : IResponse
        {
            await requestStream!.CopyToAsync(Stream.Null, token);
            return new HttpResponse(new ContentStream(responseStream, null), new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["Content-Type"] = "application/octet-stream" }, 503);
        }
    }

    private sealed class FailingDriver : INetworkDriver
    {
        public Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken token = default) where T : IResponse => throw new HttpRequestException("transport failed");
    }

    private sealed class NonSeekableStream(byte[] bytes) : MemoryStream(bytes)
    {
        public override bool CanSeek => false;
        public int BytesRead { get; private set; }
        public bool Disposed { get; private set; }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            int count = await base.ReadAsync(buffer, cancellationToken);
            BytesRead += count;
            return count;
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}