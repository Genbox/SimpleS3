using Genbox.ProviderTests.Code;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.AmazonS3.Extensions;
using Genbox.SimpleS3.Extensions.BackBlazeB2.Extensions;
using Genbox.SimpleS3.Extensions.GoogleCloudStorage.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Genbox.SimpleS3.Extensions.Wasabi.Extensions;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.ProviderTests.Buckets;

public class MiscTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task CanAccessPublicBucket(S3Provider provider, string _1, ISimpleClient authedClient)
    {
        //This tests if we can access a public bucket without credentials
        //We don't use the client provided. Instead, we build a new one.

        ServiceCollection collection = new ServiceCollection();
        collection.PostConfigure<SimpleS3Config>(config => config.Credentials = null);

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        builder.UseProfileManager()
               .BindConfigToProfile("TestSetup-" + provider);

        builder.UseHttpClientFactory();

        if (provider == S3Provider.AmazonS3)
            builder.UseAmazonS3();
        else if (provider == S3Provider.BackBlazeB2)
            builder.UseBackBlazeB2();
        else if (provider == S3Provider.GoogleCloudStorage)
            builder.UseGoogleCloudStorage();
        else if (provider == S3Provider.Wasabi)
            builder.UseWasabi();

        await using ServiceProvider services = collection.BuildServiceProvider();
        ISimpleClient client = services.GetRequiredService<ISimpleClient>();

        //Use the authenticated client to create the bucket, but use the non-authenticated client to put/get object
        await CreateTempBucketAsync(provider, authedClient, async tempBucket =>
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

            PutPublicAccessBlockResponse resp1 = await authedClient.PutPublicAccessBlockAsync(tempBucket, request =>
            {
                request.BlockPublicAcls = false;
                request.BlockPublicPolicy = false;
                request.IgnorePublicAcls = false;
                request.RestrictPublicBuckets = false;
            });
            Assert.True(resp1.IsSuccess);

            PutBucketPolicyResponse resp2 = await authedClient.PutBucketPolicyAsync(tempBucket, policy);
            Assert.True(resp2.IsSuccess);

            PutObjectResponse resp3 = await client.PutObjectStringAsync(tempBucket, "test", "test");
            Assert.True(resp3.IsSuccess);

            GetObjectResponse resp4 = await client.GetObjectAsync(tempBucket, "test");
            Assert.True(resp4.IsSuccess);
        });
    }
}