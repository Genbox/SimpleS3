using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
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

                ISimpleS3Client client = provider.GetRequiredService<ISimpleS3Client>();

                await foreach (S3Bucket bucket in client.ListAllBucketsAsync())
                {
                    if (!bucket.Name.StartsWith("testbucket-", StringComparison.OrdinalIgnoreCase))
                        continue;

                    Console.Write(bucket.Name);

                    int errors = 0;

                    IAsyncEnumerable<S3DeleteError> delAllResp = client.DeleteAllObjectsAsync(bucket.Name, true);

                    await foreach (S3DeleteError error in delAllResp)
                    {
                        errors++;

                        PutObjectLegalHoldResponse legalResp = await client.PutObjectLegalHoldAsync(bucket.Name, error.ObjectKey, false, request => request.VersionId = error.VersionId);

                        if (legalResp.IsSuccess)
                        {
                            DeleteObjectResponse delResp = await client.DeleteObjectAsync(bucket.Name, error.ObjectKey, error.VersionId);

                            if (delResp.IsSuccess)
                                errors--;
                        }
                    }

                    if (errors == 0)
                    {
                        Console.Write(" [x] emptied ");

                        DeleteBucketResponse delBucketResp = await client.DeleteBucketAsync(bucket.Name).ConfigureAwait(false);

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
    }
}