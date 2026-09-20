using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

[SuppressMessage("Usage", "IDISP017", Justification = "The tests dispose explicitly to verify stream ownership and post-disposal behavior.")]
public class DefaultResponseHandlerTests
{
    [Fact]
    public async Task HandleResponseAsyncPropagatesCancellationWhileMappingErrorBody()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        await cts.CancelAsync();

        using ILoggerFactory loggerFactory = new LoggerFactory();
        await using CancelingReadStream innerStream = new CancelingReadStream(cts.Token);
        await using ContentStream content = new ContentStream(innerStream, null);
        DefaultResponseHandler handler = new DefaultResponseHandler(Options.Create(new SimpleS3Config()),
            new NoopRequestValidatorFactory(),
            new NoopMarshalFactory(),
            new NoopPostMapperFactory(),
            new StubNetworkDriver(content, 400),
            loggerFactory.CreateLogger<DefaultResponseHandler>());

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.HandleResponseAsync<ListObjectsRequest, ListObjectsResponse>(new ListObjectsRequest("bucket"), "https://example.com", null, cts.Token));
    }

    [Fact]
    public async Task HandleResponseAsyncTreatsRedirectAsFailure()
    {
        using ILoggerFactory loggerFactory = new LoggerFactory();
        DefaultResponseHandler handler = new DefaultResponseHandler(Options.Create(new SimpleS3Config()),
            new NoopRequestValidatorFactory(),
            new NoopMarshalFactory(),
            new NoopPostMapperFactory(),
            new StubNetworkDriver(null, 302),
            loggerFactory.CreateLogger<DefaultResponseHandler>());

        ListObjectsResponse response = await handler.HandleResponseAsync<ListObjectsRequest, ListObjectsResponse>(new ListObjectsRequest("bucket"), "https://example.com", null, TestContext.Current.CancellationToken);

        Assert.False(response.IsSuccess);
        Assert.Equal(302, response.StatusCode);
    }

    [Fact]
    public async Task HandleResponseAsyncDisposesSuccessfulBufferedContent()
    {
        using ILoggerFactory loggerFactory = new LoggerFactory();
        await using TrackingStream innerStream = new TrackingStream();
        await using ContentStream content = new ContentStream(innerStream, 0);
        DefaultResponseHandler handler = CreateHandler(loggerFactory, new SuccessMarshalFactory(), new StubNetworkDriver(content, 200));

        ListObjectsResponse response = await handler.HandleResponseAsync<ListObjectsRequest, ListObjectsResponse>(new ListObjectsRequest("bucket"), "https://example.com", null, TestContext.Current.CancellationToken);

        Assert.True(response.IsSuccess);
        Assert.True(innerStream.IsDisposed);
    }

    [Fact]
    public async Task HandleResponseAsyncTransfersSuccessfulStreamingContent()
    {
        using ILoggerFactory loggerFactory = new LoggerFactory();
        await using TrackingStream innerStream = new TrackingStream();
        await using ContentStream content = new ContentStream(innerStream, 0);
        DefaultResponseHandler handler = CreateHandler(loggerFactory, new SuccessMarshalFactory(), new StubNetworkDriver(content, 200));

        using GetObjectResponse response = await handler.HandleResponseAsync<GetObjectRequest, GetObjectResponse>(new GetObjectRequest("bucket", "key"), "https://example.com", null, TestContext.Current.CancellationToken);

        Assert.False(innerStream.IsDisposed);
        Assert.Same(content, response.Content);
        response.Dispose();
        Assert.True(innerStream.IsDisposed);
    }

    [Fact]
    public async Task HandleResponseAsyncTransfersSuccessfulCustomContentResponse()
    {
        using ILoggerFactory loggerFactory = new LoggerFactory();
        await using TrackingStream innerStream = new TrackingStream();
        await using ContentStream content = new ContentStream(innerStream, 0);
        DefaultResponseHandler handler = CreateHandler(loggerFactory, new SuccessMarshalFactory(), new StubNetworkDriver(content, 200));

        using CustomContentResponse response = await handler.HandleResponseAsync<GetObjectRequest, CustomContentResponse>(new GetObjectRequest("bucket", "key"), "https://example.com", null, TestContext.Current.CancellationToken);

        Assert.False(innerStream.IsDisposed);
        Assert.Same(content, response.Content);
        response.Dispose();
        Assert.True(innerStream.IsDisposed);
    }

    [Fact]
    public async Task HandleResponseAsyncDisposesBufferedContentWhenPostMappingThrows()
    {
        using ILoggerFactory loggerFactory = new LoggerFactory();
        await using TrackingStream innerStream = new TrackingStream();
        await using ContentStream content = new ContentStream(innerStream, 0);
        DefaultResponseHandler handler = new DefaultResponseHandler(Options.Create(new SimpleS3Config()), new NoopRequestValidatorFactory(), new SuccessMarshalFactory(), new ThrowingPostMapperFactory(), new StubNetworkDriver(content, 200), loggerFactory.CreateLogger<DefaultResponseHandler>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleResponseAsync<ListObjectsRequest, ListObjectsResponse>(new ListObjectsRequest("bucket"), "https://example.com", null, TestContext.Current.CancellationToken));

        Assert.True(innerStream.IsDisposed);
    }

    [Fact]
    public async Task HandleResponseAsyncDisposesBufferedContentWhenMarshallingThrows()
    {
        using ILoggerFactory loggerFactory = new LoggerFactory();
        await using TrackingStream innerStream = new TrackingStream();
        await using ContentStream content = new ContentStream(innerStream, 0);
        DefaultResponseHandler handler = CreateHandler(loggerFactory, new ThrowingMarshalFactory(), new StubNetworkDriver(content, 200));

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleResponseAsync<ListObjectsRequest, ListObjectsResponse>(new ListObjectsRequest("bucket"), "https://example.com", null, TestContext.Current.CancellationToken));

        Assert.True(innerStream.IsDisposed);
    }

    [Fact]
    public async Task HandleResponseAsyncRecognizesConnectionClose()
    {
        using ILoggerFactory loggerFactory = new LoggerFactory();
        DefaultResponseHandler handler = new DefaultResponseHandler(Options.Create(new SimpleS3Config()),
            new NoopRequestValidatorFactory(),
            new SuccessMarshalFactory(),
            new NoopPostMapperFactory(),
            new StubNetworkDriver(null, 200, new Dictionary<string, string>(StringComparer.Ordinal) { ["connection"] = "close" }),
            loggerFactory.CreateLogger<DefaultResponseHandler>());

        ListObjectsResponse response = await handler.HandleResponseAsync<ListObjectsRequest, ListObjectsResponse>(new ListObjectsRequest("bucket"), "https://example.com", null, TestContext.Current.CancellationToken);

        Assert.True(response.ConnectionClosed);
    }

    private static DefaultResponseHandler CreateHandler(ILoggerFactory loggerFactory, IMarshalFactory marshalFactory, INetworkDriver networkDriver) => new DefaultResponseHandler(Options.Create(new SimpleS3Config()), new NoopRequestValidatorFactory(), marshalFactory, new NoopPostMapperFactory(), networkDriver, loggerFactory.CreateLogger<DefaultResponseHandler>());

    private sealed class StubNetworkDriver(ContentStream? content, int statusCode, IDictionary<string, string>? headers = null) : INetworkDriver
    {
        public Task<HttpResponse> SendRequestAsync<T>(IRequest request, string url, Stream? requestStream, CancellationToken token = default) where T : IResponse => Task.FromResult(new HttpResponse(content, headers ?? new Dictionary<string, string>(StringComparer.Ordinal), statusCode));
    }

    private sealed class SuccessMarshalFactory : IMarshalFactory
    {
        public Stream? MarshalRequest<TRequest>(SimpleS3Config config, TRequest request) where TRequest : IRequest => throw new NotSupportedException();

        public void MarshalResponse<TResponse>(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream) where TResponse : IResponse
        {
            if (response is GetObjectResponse getObjectResponse)
                getObjectResponse.Content = responseStream;
            else if (response is CustomContentResponse customResponse)
                customResponse.Content = responseStream;
        }
    }

    private sealed class CustomContentResponse : BaseResponse, IHasContent
    {
        public ContentStream Content { get; set; } = ContentStream.Null;

        [SuppressMessage("Usage", "IDISP007", Justification = "The response owns content assigned by the response handler.")]
        public void Dispose() => Content.Dispose();
    }

    private sealed class NoopMarshalFactory : IMarshalFactory
    {
        public Stream? MarshalRequest<TRequest>(SimpleS3Config config, TRequest request) where TRequest : IRequest => throw new NotSupportedException();
        public void MarshalResponse<TResponse>(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream) where TResponse : IResponse => throw new NotSupportedException();
    }

    private sealed class ThrowingMarshalFactory : IMarshalFactory
    {
        public Stream? MarshalRequest<TRequest>(SimpleS3Config config, TRequest request) where TRequest : IRequest => throw new NotSupportedException();
        public void MarshalResponse<TResponse>(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream) where TResponse : IResponse => throw new InvalidOperationException();
    }

    private sealed class NoopPostMapperFactory : IPostMapperFactory
    {
        public void PostMap<TRequest, TResponse>(SimpleS3Config config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse {}
    }

    private sealed class ThrowingPostMapperFactory : IPostMapperFactory
    {
        public void PostMap<TRequest, TResponse>(SimpleS3Config config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse => throw new InvalidOperationException();
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

    private sealed class TrackingStream : MemoryStream
    {
        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}