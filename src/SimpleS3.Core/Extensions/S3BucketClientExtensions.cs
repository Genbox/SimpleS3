using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class S3BucketClientExtensions
    {
        /// <summary>Delete a bucket</summary>
        public static Task<DeleteBucketResponse> DeleteBucketAsync(this IS3BucketClient client, string bucketName, CancellationToken token = default)
        {
            //Note: This method only exists to give a cleaner API. It provides no extra functionality.

            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            return client.DeleteBucketAsync(bucketName, null, token);
        }

        /// <summary>Create a bucket</summary>
        public static Task<CreateBucketResponse> CreateBucketAsync(this IS3BucketClient client, string bucketName, CancellationToken token = default)
        {
            //Note: This method only exists to give a cleaner API. It provides no extra functionality.

            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            return client.CreateBucketAsync(bucketName, null, token);
        }

        /// <summary>List all multipart uploads</summary>
        public static async IAsyncEnumerable<S3Upload> ListAllMultipartUploadsAsync(this IS3BucketClient client, string bucketName, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));

            string uploadIdMarker = null;
            ListMultipartUploadsResponse response;

            do
            {
                if (token.IsCancellationRequested)
                    break;

                string marker = uploadIdMarker;
                response = await client.ListMultipartUploadsAsync(bucketName, req => req.UploadIdMarker = marker, token).ConfigureAwait(false);

                foreach (S3Upload responseObject in response.Uploads)
                    yield return responseObject;

                uploadIdMarker = response.NextUploadIdMarker;
            } while (response.IsTruncated);
        }

        /// <summary>List all buckets</summary>
        public static async IAsyncEnumerable<S3Bucket> ListAllBucketsAsync(this IS3BucketClient client, Action<ListBucketsRequest> config = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            ListBucketsResponse response = await client.ListBucketsAsync(config, token).ConfigureAwait(false);

            if (!response.IsSuccess)
                throw new Exception($"Request failed with status code {response.StatusCode}");

            if (token.IsCancellationRequested)
                yield break;

            foreach (S3Bucket bucket in response.Buckets)
            {
                if (token.IsCancellationRequested)
                    yield break;

                yield return bucket;
            }
        }
    }
}