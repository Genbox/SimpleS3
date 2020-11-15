using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Utility.TestCleanup
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            S3Provider selectedProvider = UtilityHelper.SelectProvider();

            Console.WriteLine();

            string profileName = UtilityHelper.GetProfileName(selectedProvider);

            Console.WriteLine("This program will delete all buckets beginning with 'testbucket-'. Are you sure? Y/N");

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.KeyChar != 'y')
                return;

            using (ServiceProvider provider = UtilityHelper.CreateSimpleS3(selectedProvider, profileName))
            {
                UtilityHelper.GetOrSetupProfile(provider, selectedProvider, profileName);

                S3Client client = provider.GetRequiredService<S3Client>();

                await foreach (S3Bucket bucket in client.ListAllBucketsAsync())
                {
                    if (!bucket.Name.StartsWith("testbucket-", StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.WriteLine("Deleting content of " + bucket.Name);

                    DeleteAllObjectsStatus objDelResp = await client.DeleteAllObjectsAsync(bucket.Name).ConfigureAwait(false);

                    if (objDelResp == DeleteAllObjectsStatus.Ok)
                    {
                        Console.WriteLine("Deleting bucket " + bucket.Name);

                        await client.DeleteBucketAsync(bucket.Name).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}