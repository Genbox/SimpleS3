using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.Code;

public class EndpointBuilderTests
{
    [Theory]
    [InlineData("{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com", "testbucket", "https://testbucket.s3.eu-west-1.amazonaws.com")] //Test standard template for AWS
    [InlineData("{scheme}://{BUCKET:.}s3.{reGion:.}amazonaws.com", "testbucket", "https://testbucket.s3.eu-west-1.amazonaws.com")] //Test that it is case insensitive
    [InlineData("{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com", null, "https://s3.eu-west-1.amazonaws.com")] //Test that bucket is removed correctly for requests that does not support buckets
    [InlineData("{Scheme}://{Bucket:.}s3.amazonaws.com", null, "https://s3.amazonaws.com")] //Test that we don't need all labels
    [InlineData("http://s3.amazonaws.com", null, "http://s3.amazonaws.com")] //Test that we can hard code scheme and have no labels
    [InlineData("{:Scheme}://{:Bucket:}.s3.{Region:}.amazonaws.com", "testbucket", "https://testbucket.s3.eu-west-1.amazonaws.com")] //Test that we support empty pre-and post-fixes
    [InlineData("{some:Scheme}://{test-:Bucket:-production}.s3.{Region}.amazonaws.com", "testbucket", "somehttps://test-testbucket-production.s3.eu-west-1.amazonaws.com")] //Test that we have flexibility in pre- and post-fixes
    [InlineData("http://s3.{Region}.{Region}-amazonaws.com", null, "http://s3.eu-west-1.eu-west-1-amazonaws.com")] //Test that we can use a label twice
    [InlineData("{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com", "testbucket", "http://testbucket.s3.eu-west-1.amazonaws.com", false)]
    [InlineData("{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com", null, "http://s3.eu-west-1.amazonaws.com", false)]
    [InlineData("https://s3.amazonaws.com", null, "https://s3.amazonaws.com", false)]
    public void ParseEndpointTest(string template, string? bucketName, string result, bool useTls = true)
    {
        SimpleS3Config config = new SimpleS3Config(null, template, "eu-west-1");
        config.UseTls = useTls;
        config.NamingMode = NamingMode.VirtualHost;

        EndpointBuilder builder = new EndpointBuilder(Options.Create(config));

        IRequest req;

        if (bucketName == null)
            req = new ListBucketsRequest();
        else
            req = new GetObjectRequest(bucketName, null!);

        IEndpointData data = builder.GetEndpoint(req);
        Assert.Equal(result, data.Endpoint);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CachedEndpointReflectsSchemeAndNamingChanges(bool changeScheme)
    {
        SimpleS3Config config = new SimpleS3Config(null, "{Scheme}://{Bucket:.}s3.{Region}.amazonaws.com", "eu-west-1");
        EndpointBuilder builder = new EndpointBuilder(Options.Create(config));
        GetObjectRequest request = new GetObjectRequest("bucket", "key");
        IEndpointData original = builder.GetEndpoint(request);
        Assert.Equal("https://bucket.s3.eu-west-1.amazonaws.com", original.Endpoint);
        if (changeScheme)
            config.UseTls = false;
        else
            config.NamingMode = NamingMode.PathStyle;
        IEndpointData changed = builder.GetEndpoint(request);
        Assert.Equal(changeScheme ? "http://bucket.s3.eu-west-1.amazonaws.com" : "https://s3.eu-west-1.amazonaws.com", changed.Endpoint);
        Assert.Equal(changeScheme ? "bucket.s3.eu-west-1.amazonaws.com" : "s3.eu-west-1.amazonaws.com", changed.Host);
        config.UseTls = true;
        config.NamingMode = NamingMode.VirtualHost;
        Assert.Same(original, builder.GetEndpoint(request));
    }

    [Fact]
    public void CachedEndpointReflectsProviderChanges()
    {
        SimpleS3Config config = new SimpleS3Config(null, "https://s3.amazonaws.com", "cn-north-1");
        EndpointBuilder builder = new EndpointBuilder(Options.Create(config));
        ListBucketsRequest request = new ListBucketsRequest();

        IEndpointData original = builder.GetEndpoint(request);
        Assert.Equal("https://s3.amazonaws.com", original.Endpoint);

        config.ProviderName = "AmazonS3";
        Assert.Equal("https://s3.amazonaws.com.cn", builder.GetEndpoint(request).Endpoint);

        config.ProviderName = string.Empty;
        Assert.Same(original, builder.GetEndpoint(request));
    }

    [Fact]
    public async Task ConcurrentVirtualHostRequestsUseTheirOwnBucket()
    {
        SimpleS3Config config = new SimpleS3Config(null, "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com", "eu-west-1");
        config.NamingMode = NamingMode.VirtualHost;
        EndpointBuilder builder = new EndpointBuilder(Options.Create(config));

        async Task AssertEndpointAsync(string bucket)
        {
            await Task.Yield();

            for (int i = 0; i < 10_000; i++)
            {
                IEndpointData data = builder.GetEndpoint(new GetObjectRequest(bucket, "object"));
                Assert.Equal($"https://{bucket}.s3.eu-west-1.amazonaws.com", data.Endpoint);
                Assert.Equal($"{bucket}.s3.eu-west-1.amazonaws.com", data.Host);
            }
        }

        await Task.WhenAll(AssertEndpointAsync("bucket-a"),
            AssertEndpointAsync("bucket-b"),
            AssertEndpointAsync("bucket-c"),
            AssertEndpointAsync("bucket-d"));
    }

    [Fact]
    public void AlternatingBucketsRetainTheirCachedEndpoints()
    {
        SimpleS3Config config = new SimpleS3Config(null, "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com", "eu-west-1") { NamingMode = NamingMode.VirtualHost };
        EndpointBuilder builder = new EndpointBuilder(Options.Create(config));
        GetObjectRequest firstRequest = new GetObjectRequest("bucket-a", "object");
        GetObjectRequest secondRequest = new GetObjectRequest("bucket-b", "object");

        IEndpointData first = builder.GetEndpoint(firstRequest);
        _ = builder.GetEndpoint(secondRequest);
        IEndpointData firstAgain = builder.GetEndpoint(firstRequest);

        Assert.Same(first, firstAgain);
    }

    [Theory]
    [InlineData("https://s3.example.test?fixed=value")]
    [InlineData("https://s3.example.test#fragment")]
    public void RejectsEndpointQueryAndFragment(string endpoint)
    {
        SimpleS3Config config = new SimpleS3Config(null, endpoint, "eu-west-1");
        EndpointBuilder builder = new EndpointBuilder(Options.Create(config));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => builder.GetEndpoint(new ListBucketsRequest()));

        Assert.Contains("cannot contain a query string or fragment", exception.Message, StringComparison.Ordinal);
    }
}