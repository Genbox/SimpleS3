using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.TestCleanup
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            S3Provider s3Provider = UtilityHelper.SelectProvider();

            Console.WriteLine();

            string profileName = UtilityHelper.GetProfileName(s3Provider);

            Console.WriteLine("This program will delete all buckets beginning with 'testbucket-'. Are you sure? Y/N");

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.KeyChar != 'y')
                return;

            using ServiceProvider provider = UtilityHelper.CreateSimpleS3(s3Provider, profileName, true);

            IProfile profile = UtilityHelper.GetOrSetupProfile(provider, s3Provider, profileName);

            ISimpleClient client = provider.GetRequiredService<ISimpleClient>();

            await foreach (S3Bucket bucket in ListAllBucketsAsync(client))
            {
                if (!UtilityHelper.IsTestBucket(bucket.BucketName, profile) && !UtilityHelper.IsTemporaryBucket(bucket.BucketName))
                    continue;

                Console.Write(bucket.BucketName);

                int errors = await UtilityHelper.ForceDeleteBucketAsync(s3Provider, client, bucket.BucketName);

                if (errors == 0)
                {
                    Console.Write(" [x] emptied ");

                    DeleteBucketResponse delBucketResp = await client.DeleteBucketAsync(bucket.BucketName).ConfigureAwait(false);

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

        private static async IAsyncEnumerable<S3Bucket> ListAllBucketsAsync(IBucketClient client)
        {
            ListBucketsResponse response = await client.ListBucketsAsync().ConfigureAwait(false);

            foreach (S3Bucket bucket in response.Buckets)
            {
                yield return bucket;
            }
        }
    }
}