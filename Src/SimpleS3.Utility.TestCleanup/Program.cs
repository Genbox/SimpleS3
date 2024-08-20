using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.TestCleanup;

internal static class Program
{
    private static async Task Main()
    {
        S3Provider[] providers = UtilityHelper.SelectProviders().ToArray();

        Console.WriteLine();

        Console.WriteLine($"This action will delete all buckets beginning with 'testbucket-' in {providers.Length} providers");
        Console.WriteLine("Are you sure? Y/N");

        ConsoleKeyInfo key = Console.ReadKey(true);

        while (key.KeyChar != 'y' && key.KeyChar != 'n')
        {
            Console.WriteLine($"Invalid choice '{key.KeyChar}'. Try again.");
            key = Console.ReadKey(true);
        }

        if (key.KeyChar == 'n')
            return;

        Console.WriteLine();

        foreach (S3Provider s3Provider in providers)
        {
            string profileName = UtilityHelper.GetProfileName(s3Provider);
            await using ServiceProvider provider = UtilityHelper.CreateSimpleS3(s3Provider, profileName, true);

            IProfile profile = UtilityHelper.GetOrSetupProfile(provider, s3Provider, profileName);

            ISimpleClient client = provider.GetRequiredService<ISimpleClient>();

            List<S3Bucket> buckets = await ListAllBucketsAsync(client).Where(x => UtilityHelper.IsTestBucket(x.BucketName, profile) || UtilityHelper.IsTemporaryBucket(x.BucketName)).ToListAsync();

            Console.WriteLine($"# {s3Provider}: {buckets.Count} bucket(s)");

            foreach (S3Bucket bucket in buckets)
            {
                string bucketName = bucket.BucketName;

                Console.Write(bucketName);

                int errors = await UtilityHelper.ForceEmptyBucketAsync(s3Provider, client, bucketName);

                if (errors == 0)
                {
                    Console.Write(" [x] emptied ");

                    DeleteBucketResponse delBucketResp = await client.DeleteBucketAsync(bucketName).ConfigureAwait(false);

                    if (delBucketResp.IsSuccess)
                        Console.Write("[x] deleted");
                    else
                        Console.Write("[ ] deleted");
                }
                else
                    Console.Write(" [ ] emptied [ ] deleted");

                Console.WriteLine();
            }
        }
    }

    private static async IAsyncEnumerable<S3Bucket> ListAllBucketsAsync(IBucketClient client)
    {
        ListBucketsResponse response = await client.ListBucketsAsync().ConfigureAwait(false);

        foreach (S3Bucket bucket in response.Buckets)
            yield return bucket;
    }
}