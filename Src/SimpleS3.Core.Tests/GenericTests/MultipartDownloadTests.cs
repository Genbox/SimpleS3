using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Fluent;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class MultipartDownloadTests
{
    [Fact]
    public async Task PreservesPrefixAndUsesSelectedVersionPartSizes()
    {
        IObjectClient client = CreateClient(out RecordingObjectClient recording);
        MultipartTransfer transfer = new MultipartTransfer(client, null!, null!, []);
        await using MemoryStream output = new MemoryStream();
        output.WriteByte(99);

        await foreach (GetObjectResponse response in transfer.MultipartDownloadAsync("bucket", "key", output, config: req => req.VersionId = "old", token: TestContext.Current.CancellationToken))
            response.Dispose();

        Assert.Equal(new byte[] { 99, 1, 2, 3 }, output.ToArray());
        Assert.All(recording.Heads, req => Assert.Equal("old", req.VersionId));
        Assert.All(recording.Gets, req => Assert.Equal("old", req.VersionId));
    }

    [Theory]
    [InlineData("selected")]
    [InlineData(null)]
    [InlineData("null")]
    public async Task PinsSubsequentRequestsToInitialObject(string? version)
    {
        IObjectClient client = CreateClient(out RecordingObjectClient recording);
        recording.Version = version;
        MultipartTransfer transfer = new MultipartTransfer(client, null!, null!, []);
        await using MemoryStream output = new MemoryStream();

        await foreach (GetObjectResponse response in transfer.MultipartDownloadAsync("bucket", "key", output, token: TestContext.Current.CancellationToken))
            response.Dispose();

        foreach (HeadObjectRequest request in recording.Heads.Skip(1).Concat(recording.Gets))
        {
            Assert.Equal(version, request.VersionId);
            if (version is null or "null")
                Assert.Contains("etag", request.IfETagMatch.Build(), StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task MetadataRequestsIncludeEncryptionPayerOwnerAndConditions()
    {
        IObjectClient client = CreateClient(out RecordingObjectClient recording);
        MultipartTransfer transfer = new MultipartTransfer(client, null!, null!, []);
        await using MemoryStream output = new MemoryStream();
        byte[] key = new byte[32];
        key[0] = 42;

        await foreach (GetObjectResponse response in transfer.MultipartDownloadAsync("bucket", "key", output, config: req =>
                       {
                           req.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                           req.SseCustomerKey = key;
                           req.SseCustomerKeyMd5 = [7];
                           req.RequestPayer = Payer.Requester;
                           req.ExpectedBucketOwner = "owner";
                           req.IfETagMatch.Set("etag");
                       }, token: TestContext.Current.CancellationToken))
            response.Dispose();

        Assert.All(recording.Heads, req =>
        {
            Assert.Equal(key, req.SseCustomerKey);
            Assert.Equal(SseCustomerAlgorithm.Aes256, req.SseCustomerAlgorithm);
            Assert.Equal(new byte[] { 7 }, req.SseCustomerKeyMd5);
            Assert.Equal(Payer.Requester, req.RequestPayer);
            Assert.Equal("owner", req.ExpectedBucketOwner);
            Assert.Contains("etag", req.IfETagMatch.Build(), StringComparison.Ordinal);
        });
        Assert.Equal(42, key[0]);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(2, true)]
    public async Task ThrowsOnUnsuccessfulMetadataOrPart(int part, bool get)
    {
        IObjectClient client = CreateClient(out RecordingObjectClient recording);
        recording.FailedHead = get ? null : part;
        recording.FailedGet = get ? part : null;
        MultipartTransfer transfer = new MultipartTransfer(client, null!, null!, []);
        await using MemoryStream output = new MemoryStream();

        await Assert.ThrowsAsync<S3RequestException>(async () =>
        {
            await foreach (GetObjectResponse response in transfer.MultipartDownloadAsync("bucket", "key", output, token: TestContext.Current.CancellationToken))
                response.Dispose();
        });
    }

    private static IObjectClient CreateClient(out RecordingObjectClient recording)
    {
        IObjectClient client = DispatchProxy.Create<IObjectClient, RecordingObjectClient>();
        recording = (RecordingObjectClient)client;
        return client;
    }

    private class RecordingObjectClient : DispatchProxy
    {
        public List<HeadObjectRequest> Heads { get; } = [];
        public List<GetObjectRequest> Gets { get; } = [];
        public string? Version { get; set; } = "selected";
        public int? FailedHead { get; set; }
        public int? FailedGet { get; set; }

        [SuppressMessage("Usage", "IDISP004", Justification = "The response owns its content stream and the download disposes it.")]
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod!.Name == nameof(IObjectClient.HeadObjectAsync))
            {
                HeadObjectRequest request = new HeadObjectRequest("bucket", "key");
                ((Action<HeadObjectRequest>?)args![2])?.Invoke(request);
                Heads.Add(request);
                return Task.FromResult(new HeadObjectResponse
                {
                    IsSuccess = request.PartNumber != FailedHead,
                    NumberOfParts = 2,
                    ContentLength = request.VersionId == "old" && request.PartNumber == 1 ? 2 : 1,
                    VersionId = request.VersionId ?? Version,
                    ETag = "\"etag\""
                });
            }

            if (targetMethod.Name == nameof(IObjectClient.GetObjectAsync))
            {
                GetObjectRequest request = new GetObjectRequest("bucket", "key");
                ((Action<GetObjectRequest>?)args![2])?.Invoke(request);
                Gets.Add(request);
                byte[] bytes = request.VersionId == "old" ? [1, 2] : [1];
                if (request.PartNumber != 1)
                    bytes = [3];
                bool failed = request.PartNumber == FailedGet;
                return Task.FromResult(new GetObjectResponse
                {
                    IsSuccess = !failed,
                    StatusCode = failed ? 403 : 206,
                    Content = failed ? ContentStream.Null : new ContentStream(new MemoryStream(bytes), bytes.Length)
                });
            }

            throw new NotSupportedException();
        }
    }
}