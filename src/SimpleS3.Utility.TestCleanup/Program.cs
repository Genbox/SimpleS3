using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
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
            S3Provider selectedProvider = UtilityHelper.SelectProvider();

            Console.WriteLine();

            string profileName = UtilityHelper.GetProfileName(selectedProvider);

            Console.WriteLine("This program will delete all buckets beginning with 'testbucket-'. Are you sure? Y/N");

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.KeyChar != 'y')
                return;

            using ServiceProvider provider = UtilityHelper.CreateSimpleS3(selectedProvider, profileName);

            IProfile profile = UtilityHelper.GetOrSetupProfile(provider, selectedProvider, profileName);

            ISimpleClient client = provider.GetRequiredService<ISimpleClient>();

            await foreach (S3Bucket bucket in ListAllBucketsAsync(client))
            {
                if (!UtilityHelper.IsTestBucket(bucket.BucketName, profile) && !UtilityHelper.IsTemporaryBucket(bucket.BucketName))
                    continue;

                Console.Write(bucket.BucketName);

                int errors = await EmptyBucketAsync(client, bucket.BucketName);

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

        private static async Task<int> EmptyBucketAsync(ISimpleClient client, string bucketName)
        {
            int errors = 0;

            IAsyncEnumerable<S3DeleteError> delAllResp = DeleteAllObjects(client, bucketName);

            await foreach (S3DeleteError error in delAllResp)
            {
                errors++;

                PutObjectLegalHoldResponse legalResp = await client.PutObjectLegalHoldAsync(bucketName, error.ObjectKey, false, request => request.VersionId = error.VersionId);

                if (legalResp.IsSuccess)
                {
                    DeleteObjectResponse delResp = await client.DeleteObjectAsync(bucketName, error.ObjectKey, x => x.VersionId = error.VersionId);

                    if (delResp.IsSuccess)
                        errors--;
                }
            }

            //Abort all incomplete multipart uploads
            IAsyncEnumerable<S3Upload> partUploads = client.ListAllMultipartUploadsAsync(bucketName);

            await foreach (S3Upload partUpload in partUploads)
            {
                await client.AbortMultipartUploadAsync(bucketName, partUpload.ObjectKey, partUpload.UploadId);
            }

            return errors;
        }

        private static async IAsyncEnumerable<S3Bucket> ListAllBucketsAsync(IBucketClient client)
        {
            ListBucketsResponse response = await client.ListBucketsAsync().ConfigureAwait(false);

            foreach (S3Bucket bucket in response.Buckets)
            {
                yield return bucket;
            }
        }

        private static async IAsyncEnumerable<S3DeleteError> DeleteAllObjects(ISimpleClient client, string bucket)
        {
            ListObjectVersionsResponse response;
            Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucket);

            do
            {
                response = await responseTask;

                if (!response.IsSuccess)
                    yield break;

                if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                    break;

                if (response.IsTruncated)
                {
                    string keyMarker = response.NextKeyMarker;
                    responseTask = client.ListObjectVersionsAsync(bucket, req => req.KeyMarker = keyMarker);
                }

                IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                           .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

                DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucket, delete, req => req.Quiet = false).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    yield break;

                foreach (S3DeleteError error in multiDelResponse.Errors)
                {
                    yield return error;
                }

            } while (response.IsTruncated);
        }
    }
}