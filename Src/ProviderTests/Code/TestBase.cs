using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Code;

public abstract class TestBase
{
    protected static string GetTemporaryBucket() => UtilityHelper.GetTemporaryBucket();

    protected static async Task AllowSseCustomerKeysAsync(S3Provider provider, ISimpleClient client, string bucketName)
    {
        if (provider != S3Provider.AmazonS3)
            return;

        S3ServerSideEncryptionRule rule = new S3ServerSideEncryptionRule(SseAlgorithm.Aes256);
        rule.BlockedEncryptionTypes.Add(BlockedEncryptionType.None);

        PutBucketEncryptionResponse resp = await client.PutBucketEncryptionAsync(bucketName, [rule]);
        Assert.Equal(200, resp.StatusCode);
    }

    protected static async Task CreateTempBucketAsync(S3Provider provider, ISimpleClient client, Func<string, Task> action, Action<CreateBucketRequest>? config = null)
    {
        string tempBucket = GetTemporaryBucket();

        CreateBucketResponse createResponse = await client.CreateBucketAsync(tempBucket, config);
        Assert.Equal(200, createResponse.StatusCode);

        try
        {
            await action(tempBucket);
        }
        finally
        {
            int errors = await UtilityHelper.ForceEmptyBucketAsync(provider, client, tempBucket);

            if (errors == 0)
            {
                DeleteBucketResponse delBucketResp = await client.DeleteBucketAsync(tempBucket);
                Assert.True(delBucketResp.IsSuccess);
            }

            Assert.Equal(0, errors);
        }
    }
}
