using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.TestSetup;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("You are about to set up the test environment for the provider tests");

        foreach (S3Provider s3Provider in UtilityHelper.SelectProviders())
        {
            Console.WriteLine();

            string profileName = UtilityHelper.GetProfileName(s3Provider);

            using ServiceProvider provider = UtilityHelper.CreateSimpleS3(s3Provider, profileName, true);

            IProfile profile = UtilityHelper.GetOrSetupProfile(provider, s3Provider, profileName);

            IBucketClient bucketClient = provider.GetRequiredService<IBucketClient>();

            string bucketName = UtilityHelper.GetTestBucket(profile);
            Console.WriteLine($"Setting up bucket '{bucketName}' in {s3Provider}");

            if (await TryCreateBucketAsync(bucketClient, bucketName))
            {
                if (s3Provider == S3Provider.AmazonS3)
                    await PostConfigureAmazonS3(bucketClient, bucketName).ConfigureAwait(false);
                else if (s3Provider == S3Provider.BackBlazeB2)
                    await PostConfigureBackBlazeB2(bucketClient, bucketName).ConfigureAwait(false);
            }
        }
    }

    private static async Task PostConfigureBackBlazeB2(IBucketClient bucketClient, string bucketName)
    {
        await SetLockConfigAsync(bucketClient, bucketName);
    }

    private static async Task PostConfigureAmazonS3(IBucketClient bucketClient, string bucketName)
    {
        await SetLockConfigAsync(bucketClient, bucketName);
        await SetExpireAllAsync(bucketClient, bucketName);
    }

    private static async Task<bool> TryCreateBucketAsync(IBucketClient bucketClient, string bucketName)
    {
        HeadBucketResponse headResp = await bucketClient.HeadBucketAsync(bucketName);

        Console.Write("- Created: ");

        //Bucket already exist - we return true to apply post-configuration
        if (headResp.StatusCode == 200)
        {
            Console.WriteLine("[x]");
            return true;
        }

        CreateBucketResponse resp = await bucketClient.CreateBucketAsync(bucketName, r => r.EnableObjectLocking = true).ConfigureAwait(false);

        if (resp.IsSuccess)
        {
            Console.WriteLine("[x]");
            return true;
        }

        Console.WriteLine("[ ]");
        return false;
    }

    private static async Task SetLockConfigAsync(IBucketClient bucketClient, string bucketName)
    {
        Console.Write("- Lock configuration: ");

        PutBucketLockConfigurationResponse resp = await bucketClient.PutBucketLockConfigurationAsync(bucketName, true).ConfigureAwait(false);
        Console.WriteLine(resp.IsSuccess ? "[x]" : "[ ]");
    }

    private static async Task SetExpireAllAsync(IBucketClient bucketClient, string bucketName)
    {
        Console.Write("- Lifecycle configuration: ");

        List<S3Rule> rules = new List<S3Rule>
        {
            new S3Rule("ExpireAll", true)
            {
                AbortIncompleteMultipartUploadDays = 1,
                NonCurrentVersionExpirationDays = 1,
                Expiration = new S3Expiration(1),
                Filter = new S3Filter { Prefix = "" }
            }
        };

        PutBucketLifecycleConfigurationResponse resp = await bucketClient.PutBucketLifecycleConfigurationAsync(bucketName, rules).ConfigureAwait(false);
        Console.WriteLine(resp.IsSuccess ? "[x]" : "[ ]");
    }
}