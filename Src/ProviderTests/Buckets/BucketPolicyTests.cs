using System.Text.Json.Nodes;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketPolicyTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutBucketPolicyRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        string policy = $$"""
                          {
                          	"Version": "2012-10-17",
                          	"Id": "Policy1740044169364",
                          	"Statement": [
                          		{
                          			"Sid": "Stmt1740044164489",
                          			"Effect": "Allow",
                          			"Principal": "*",
                          			"Action": "s3:*",
                          			"Resource": "arn:aws:s3:::{{tempBucket}}/*"
                          		}
                          	]
                          }
                          """;

        PutPublicAccessBlockResponse resp1 = await client.PutPublicAccessBlockAsync(tempBucket, request =>
        {
            request.BlockPublicAcls = false;
            request.BlockPublicPolicy = false;
            request.IgnorePublicAcls = false;
            request.RestrictPublicBuckets = false;
        }, TestContext.Current.CancellationToken);
        Assert.True(resp1.IsSuccess);

        PutBucketPolicyResponse putResp = await client.PutBucketPolicyAsync(tempBucket, policy, token: TestContext.Current.CancellationToken);
        Assert.Equal(204, putResp.StatusCode);

        GetBucketPolicyResponse getResp = await client.GetBucketPolicyAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.NotNull(getResp.Policy);
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(policy), JsonNode.Parse(getResp.Policy)));

        DeleteBucketPolicyResponse deleteResp = await client.DeleteBucketPolicyAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(204, deleteResp.StatusCode);

        GetBucketPolicyResponse deleted = await client.GetBucketPolicyAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(404, deleted.StatusCode);
    });
}