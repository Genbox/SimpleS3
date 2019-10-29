using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class S3BucketClientExtensions
    {
        public static Task<CreateBucketResponse> PutBucketAsync(this IS3BucketClient client, string bucketName, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            return client.CreateBucketAsync(bucketName, null, token);
        }

        /// <summary>List all objects in a bucket</summary>
        /// <param name="client">The BucketClient</param>
        /// <param name="bucketName">The name of the bucket you want to list objects in.</param>
        /// <param name="getOwnerInfo">Set to true if you want to get object owner information as well.</param>
        /// <param name="token">A cancellation token</param>
        public static async IAsyncEnumerable<S3Object> ListObjectsRecursiveAsync(this IS3BucketClient client, string bucketName, bool getOwnerInfo = false, Action<ListObjectsRequest> config = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            string continuationToken = null;
            ListObjectsResponse response;

            do
            {
                if (token.IsCancellationRequested)
                    break;

                string cToken = continuationToken;
                response = await client.ListObjectsAsync(bucketName, req =>
                {
                    req.ContinuationToken = cToken;

                    if (getOwnerInfo)
                        req.FetchOwner = true;

                    config?.Invoke(req);
                }, token).ConfigureAwait(false);

                if (!response.IsSuccess)
                    throw new Exception("Request failed");

                foreach (S3Object responseObject in response.Objects)
                    yield return responseObject;

                continuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);
        }

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

        public static async Task<DeleteBucketStatus> DeleteBucketRecursiveAsync(this IS3BucketClient client, string bucketName, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));

            EmptyBucketStatus emptyResp = await client.EmptyBucketAsync(bucketName, token).ConfigureAwait(false);

            if (emptyResp != EmptyBucketStatus.Ok)
                return DeleteBucketStatus.FailedToEmpty;

            DeleteBucketResponse delResponse = await client.DeleteBucketAsync(bucketName, null, token).ConfigureAwait(false);

            if (!delResponse.IsSuccess)
            {
                if (delResponse.Error.Code == ErrorCode.BucketNotEmpty)
                    return DeleteBucketStatus.BucketNotEmpty;

                return DeleteBucketStatus.RequestFailed;
            }

            return DeleteBucketStatus.Ok;
        }

        public static async IAsyncEnumerable<S3Bucket> ListAllBuckets(this IS3BucketClient client, Action<ListBucketsRequest> config = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));

            ListBucketsResponse resp = await client.ListBucketsAsync(config, token).ConfigureAwait(false);

            foreach (S3Bucket respBucket in resp.Buckets)
                yield return respBucket;
        }
    }
}