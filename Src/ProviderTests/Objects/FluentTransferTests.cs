using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class FluentTransferTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task CreateUploadRoundTrip(S3Provider _, string bucket, ISimpleClient client)
    {
        byte[] data = "fluent upload"u8.ToArray();
        string key = "fluent-upload-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key,
            () => client.CreateUpload(bucket, key).UploadDataAsync(data, TestContext.Current.CancellationToken),
            _ => AssertObjectContentAsync(client, bucket, key, data));
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task UploadMetadata(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "fluent-metadata-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        await TemporaryObject.WithAsync(client, bucket, key,
            () => client.CreateUpload(bucket, key).WithMetadata("purpose", "fluent-test").UploadStringAsync("payload", token: token), async _ =>
            {
                HeadObjectResponse head = await client.HeadObjectAsync(bucket, key, token: token);
                Assert.Equal(200, head.StatusCode);
                Assert.NotNull(head.Metadata);
                Assert.Equal("fluent-test", head.Metadata["purpose"]);
            });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task CreateDownloadRoundTrip(S3Provider _, string bucket, ISimpleClient client)
    {
        byte[] data = "fluent download"u8.ToArray();
        string key = "fluent-download-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectDataAsync(bucket, key, data, token: token), async _ =>
        {
            using GetObjectResponse read = await client.CreateDownload(bucket, key).DownloadAsync(token);
            Assert.Equal(200, read.StatusCode);
            Assert.Equal(data, await read.Content.AsDataAsync(token));
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task DownloadRange(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = "fluent-range-" + Guid.NewGuid().ToString("N");
        CancellationToken token = TestContext.Current.CancellationToken;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectStringAsync(bucket, key, "0123456789", token: token), async _ =>
        {
            using GetObjectResponse read = await client.CreateDownload(bucket, key).WithRange(2, 5).DownloadAsync(token);
            Assert.Equal(206, read.StatusCode);
            Assert.Equal("2345", await read.Content.AsStringAsync(token: token));
        });
    }
}