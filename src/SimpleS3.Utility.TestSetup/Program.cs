using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        S3Provider selectedProvider = UtilityHelper.SelectProvider();

        Console.WriteLine();

        string profileName = UtilityHelper.GetProfileName(selectedProvider);

        using ServiceProvider provider = UtilityHelper.CreateSimpleS3(selectedProvider, profileName, true);

        IProfile profile = UtilityHelper.GetOrSetupProfile(provider, selectedProvider, profileName);
        string bucketName = UtilityHelper.GetTestBucket(profile);

        IBucketClient bucketClient = provider.GetRequiredService<IBucketClient>();

        Console.WriteLine($"Setting up bucket '{bucketName}'");

        bool success = await TryCreateBucketAsync(bucketClient, bucketName);

        if (success)
        {
            if (selectedProvider == S3Provider.AmazonS3)
                await PostConfigureAmazonS3(bucketClient, bucketName).ConfigureAwait(false);
            else if (selectedProvider == S3Provider.BackBlazeB2)
                await PostConfigureBackBlazeB2(bucketClient, bucketName).ConfigureAwait(false);
        }
    }

    private static async Task PostConfigureBackBlazeB2(IBucketClient bucketClient, string bucketName)
    {
        Console.WriteLine("Adding lock configuration");

        PutBucketLockConfigurationResponse putLockResp = await bucketClient.PutBucketLockConfigurationAsync(bucketName, true).ConfigureAwait(false);

        if (putLockResp.IsSuccess)
            Console.WriteLine("Successfully applied lock configuration.");
        else
            Console.Error.WriteLine("Failed to apply lock configuration.");
    }

    private static async Task PostConfigureAmazonS3(IBucketClient bucketClient, string bucketName)
    {
        Console.WriteLine("Adding lock configuration");

        PutBucketLockConfigurationResponse putLockResp = await bucketClient.PutBucketLockConfigurationAsync(bucketName, true).ConfigureAwait(false);

        if (putLockResp.IsSuccess)
            Console.WriteLine("Successfully applied lock configuration.");
        else
            Console.Error.WriteLine("Failed to apply lock configuration.");

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

        Console.WriteLine("Adding lifecycle configuration");

        PutBucketLifecycleConfigurationResponse putLifecycleResp = await bucketClient.PutBucketLifecycleConfigurationAsync(bucketName, rules).ConfigureAwait(false);

        if (putLifecycleResp.IsSuccess)
            Console.WriteLine("Successfully applied lifecycle configuration.");
        else
            Console.Error.WriteLine("Failed to apply lifecycle configuration.");

    }

    private static async Task<bool> TryCreateBucketAsync(IBucketClient bucketClient, string bucketName)
    {
        HeadBucketResponse headResp = await bucketClient.HeadBucketAsync(bucketName);

        //Bucket already exist - we return true to apply post-configuration
        if (headResp.StatusCode == 200)
            return true;

        CreateBucketResponse createResp = await bucketClient.CreateBucketAsync(bucketName, r => r.EnableObjectLocking = true).ConfigureAwait(false);

        if (createResp.IsSuccess)
        {
            Console.WriteLine($"Successfully created '{bucketName}'.");
            return true;
        }

        Console.Error.WriteLine($"Failed to create '{bucketName}'. Error: " + createResp.Error?.Message);
        return false;
    }
}