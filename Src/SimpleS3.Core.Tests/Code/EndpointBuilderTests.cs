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
    public void ParseEndpointTest(string template, string? bucketName, string result)
    {
        SimpleS3Config config = new SimpleS3Config(null!, "eu-west-1");
        config.EndpointTemplate = template;
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
}