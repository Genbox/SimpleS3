using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketPolicyStatusTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, false, true)]
    public async Task GetBucketPolicyStatusRoundTrip(S3Provider provider, string _, ISimpleClient client, bool isPublic) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        Assert.Equal(200, (await client.PutPublicAccessBlockAsync(bucket, request =>
        {
            request.BlockPublicPolicy = false;
            request.RestrictPublicBuckets = false;
        }, token)).StatusCode);
        string effect = isPublic ? "Allow" : "Deny";
        string policy = $$"""
                          {"Version":"2012-10-17","Statement":[{"Effect":"{{effect}}","Principal":"*","Action":"s3:GetObject","Resource":"arn:aws:s3:::{{bucket}}/*"}]}
                          """;
        Assert.Equal(204, (await client.PutBucketPolicyAsync(bucket, policy, token: token)).StatusCode);
        GetBucketPolicyStatusResponse read = await client.GetBucketPolicyStatusAsync(bucket, token: token);
        Assert.Equal(200, read.StatusCode);
        Assert.Equal(isPublic, read.IsPublic);
    });
}