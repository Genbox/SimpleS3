using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.TestCleanup;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        if (!args.Contains("--temporary-only", StringComparer.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine("Usage: SimpleS3.Utility.TestCleanup --temporary-only");
            return 2;
        }

        S3Provider[] providers = [S3Provider.AmazonS3, S3Provider.BackBlazeB2, S3Provider.GoogleCloudStorage, S3Provider.Wasabi];
        int failures = 0;

        Console.WriteLine("Deleting buckets matching 'tempbucket-{guid}' from configured providers.");

        foreach (S3Provider s3Provider in providers)
        {
            string profileName = UtilityHelper.GetProfileName(s3Provider);
            ServiceProvider provider = UtilityHelper.CreateSimpleS3(s3Provider, profileName, true);
            await using var configuredProvider = provider.ConfigureAwait(false);
            IProfileManager profileManager = provider.GetRequiredService<IProfileManager>();

            if (profileManager.GetProfile(profileName) == null)
            {
                Console.WriteLine($"# {s3Provider}: profile not configured");
                continue;
            }

            ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
            ListBucketsResponse listResponse = await client.ListBucketsAsync(token: CancellationToken.None).ConfigureAwait(false);

            if (!listResponse.IsSuccess)
            {
                Console.WriteLine($"# {s3Provider}: unable to list buckets ({listResponse.StatusCode})");
                failures++;
                continue;
            }

            S3Bucket[] buckets = listResponse.Buckets.Where(x => UtilityHelper.IsTemporaryBucket(x.BucketName)).ToArray();

            Console.WriteLine($"# {s3Provider}: {buckets.Length} bucket(s)");

            foreach (S3Bucket bucket in buckets)
            {
                string bucketName = bucket.BucketName;

                Console.Write(bucketName);

                (Exception? failure, bool deferred) = await UtilityHelper.TryDeleteTemporaryBucketAsync(s3Provider, client, bucketName, token: CancellationToken.None).ConfigureAwait(false);
                string outcome = deferred ? " [ ] deferred: " + failure?.Message : " [ ] failed: " + failure?.Message;
                Console.WriteLine(failure == null ? " [x] deleted" : outcome);

                if (failure != null)
                    failures++;
            }
        }

        return failures == 0 ? 0 : 1;
    }
}