using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class DefaultResponseHandlerTests
{
    [Fact]
    public async Task HandleResponseAsyncPropagatesCancellationWhileMappingErrorBody()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        await cts.CancelAsync().ConfigureAwait(false);

        using ILoggerFactory loggerFactory = new LoggerFactory();
        await using ContentStream content = new ContentStream(new CancelingReadStream(cts.Token), null);
        DefaultResponseHandler handler = new DefaultResponseHandler(
            Options.Create(new SimpleS3Config()),
            new NoopRequestValidatorFactory(),
            new NoopMarshalFactory(),
            new NoopPostMapperFactory(),
            new StubNetworkDriver(content),
            loggerFactory.CreateLogger<DefaultResponseHandler>());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.HandleResponseAsync<ListObjectsRequest, ListObjectsResponse>(new ListObjectsRequest("bucket"), "https://example.com", null, cts.Token));
    }

    private sealed class StubNetworkDriver(ContentStream content) : INetworkDriver
    {
        public Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken cancellationToken = default) where T : IResponse
        {
            return Task.FromResult(new HttpResponse(content, new Dictionary<string, string>(), 400));
        }
    }

    private sealed class NoopMarshalFactory : IMarshalFactory
    {
        public Stream? MarshalRequest<TRequest>(SimpleS3Config config, TRequest request) where TRequest : IRequest => throw new NotSupportedException();
        public void MarshalResponse<TResponse>(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream) where TResponse : IResponse => throw new NotSupportedException();
    }

    private sealed class NoopPostMapperFactory : IPostMapperFactory
    {
        public void PostMap<TRequest, TResponse>(SimpleS3Config config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse {}
    }

    private sealed class NoopRequestValidatorFactory : IRequestValidatorFactory
    {
        public void ValidateAndThrow<T>(T obj) where T : IRequest {}
    }

    private sealed class CancelingReadStream(CancellationToken token) : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new OperationCanceledException(token);
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => throw new OperationCanceledException(token);
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => throw new OperationCanceledException(token);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() {}
    }
}