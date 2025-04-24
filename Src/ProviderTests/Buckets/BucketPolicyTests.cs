using Genbox.ProviderTests.Code;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketPolicyTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)] //Not supported on other cloud providers
    public async Task PutBucketPolicyRequest(S3Provider provider, string _, ISimpleClient client)
    {
        await CreateTempBucketAsync(provider, client, async tempBucket =>
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
            });
            Assert.True(resp1.IsSuccess);

            PutBucketPolicyResponse getResp = await client.PutBucketPolicyAsync(tempBucket, policy);
            Assert.Equal(204, getResp.StatusCode);

            GetBucketPolicyResponse putResp = await client.GetBucketPolicyAsync(tempBucket);
            Assert.Equal(200, putResp.StatusCode);
            Assert.NotEmpty(putResp.Policy);

            DeleteBucketPolicyResponse getResp2 = await client.DeleteBucketPolicyAsync(tempBucket);
            Assert.Equal(204, getResp2.StatusCode);

            GetBucketPolicyResponse putResp2 = await client.GetBucketPolicyAsync(tempBucket);
            Assert.Equal(404, putResp2.StatusCode);
        });
    }
}