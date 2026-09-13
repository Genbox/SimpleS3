using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketWebsiteTests : TestBase
{
    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task PutBucketWebsiteRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        S3WebsiteConfiguration configuration = new S3WebsiteConfiguration { IndexDocumentSuffix = "index.html" };
        Assert.Equal(200, (await client.PutBucketWebsiteAsync(bucket, configuration, token: token)).StatusCode);
        GetBucketWebsiteResponse read = await client.GetBucketWebsiteAsync(bucket, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal("index.html", read.WebsiteConfiguration.IndexDocumentSuffix);
        Assert.Equal(204, (await client.DeleteBucketWebsiteAsync(bucket, token: token)).StatusCode);
        Assert.Equal(404, (await client.GetBucketWebsiteAsync(bucket, token: token)).StatusCode);
    });

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task ErrorDocument(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3WebsiteConfiguration configuration = new S3WebsiteConfiguration { IndexDocumentSuffix = "index.html", ErrorDocumentKey = "error.html" };
        Assert.Equal(200, (await client.PutBucketWebsiteAsync(bucket, configuration, token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketWebsiteResponse read = await client.GetBucketWebsiteAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal("error.html", read.WebsiteConfiguration.ErrorDocumentKey);
    });
}