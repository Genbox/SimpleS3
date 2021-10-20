#if COMMERCIAL
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class ObjectClientExtensions
    {
        public static async Task<DeleteObjectResponse> DeleteObjectAsync(this IObjectClient client, string bucketName, string objectKey, string? versionId = null, MfaAuthenticationBuilder? mfa = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            DeleteObjectResponse resp = await client.DeleteObjectAsync(bucketName, objectKey, req =>
            {
                req.VersionId = versionId;

                if (mfa != null)
                    req.Mfa = mfa;
            }, token).ConfigureAwait(false);

            return resp;
        }

        public static Task<DeleteObjectsResponse> DeleteObjectsAsync(this IObjectClient client, string bucketName, IEnumerable<string> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKeys, nameof(objectKeys));

            return client.DeleteObjectsAsync(bucketName, objectKeys.Select(x => new S3DeleteInfo(x)), config, token);
        }

        /// <summary>Delete all objects within the bucket</summary>
        public static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectsAsync(this IObjectClient client, string bucketName, string? prefix = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            ListObjectsResponse response;
            Task<ListObjectsResponse> responseTask = client.ListObjectsAsync(bucketName, req => req.Prefix = prefix, token);

            ObjectPool<S3DeleteInfo> pool = ObjectPool<S3DeleteInfo>.Shared;

            do
            {
                if (token.IsCancellationRequested)
                    yield break;

                response = await responseTask;

                if (!response.IsSuccess)
                    throw new S3RequestException(response, $"Unable to list objects in bucket '{bucketName}");

                if (response.Objects.Count == 0)
                    yield break;

                if (response.IsTruncated)
                {
                    string localToken = response.NextContinuationToken;
                    responseTask = client.ListObjectsAsync(bucketName, req =>
                    {
                        req.Prefix = prefix;
                        req.ContinuationToken = localToken;
                    }, token);
                }

                IList<S3DeleteInfo> delete = response.Objects.Select(x => pool.Rent(info => info.Initialize(x.ObjectKey))).ToList();

                DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucketName, delete, req => req.Quiet = false, token).ConfigureAwait(false);

                pool.Return(delete);

                if (!multiDelResponse.IsSuccess)
                    throw new S3RequestException(response, $"Unable to delete objects in bucket '{bucketName}");

                foreach (S3DeleteError error in multiDelResponse.Errors)
                {
                    yield return error;
                }
            } while (response.IsTruncated);
        }

        public static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(this IObjectClient client, string bucketName, string? prefix = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            ListObjectVersionsResponse response;
            Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucketName, req => req.Prefix = prefix, token);

            do
            {
                if (token.IsCancellationRequested)
                    break;

                response = await responseTask;

                if (!response.IsSuccess)
                    throw new S3RequestException(response, $"Unable to list objects in bucket '{bucketName}");

                if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                    yield break;

                if (response.IsTruncated)
                {
                    string keyMarker = response.NextKeyMarker;
                    responseTask = client.ListObjectVersionsAsync(bucketName, req =>
                    {
                        req.Prefix = prefix;
                        req.KeyMarker = keyMarker;
                    }, token);
                }

                IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                           .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

                DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucketName, delete, req => req.Quiet = false, token).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    throw new S3RequestException(response, $"Unable to delete objects in bucket '{bucketName}");

                foreach (S3DeleteError error in multiDelResponse.Errors)
                {
                    yield return error;
                }
            } while (response.IsTruncated);
        }

        public static async Task<PutObjectResponse> PutObjectDataAsync(this IObjectClient client, string bucketName, string objectKey, byte[] data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            using (MemoryStream ms = new MemoryStream(data))
                return await client.PutObjectAsync(bucketName, objectKey, ms, config, token).ConfigureAwait(false);
        }

        public static Task<PutObjectResponse> PutObjectStringAsync(this IObjectClient client, string bucketName, string objectKey, string content, Encoding? encoding = null, Action<PutObjectRequest>? config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            encoding ??= Constants.Utf8NoBom;

            return client.PutObjectDataAsync(bucketName, objectKey, encoding.GetBytes(content), config, token);
        }

        public static async Task<PutObjectResponse> PutObjectFileAsync(this IObjectClient client, string bucketName, string objectKey, string file, Action<PutObjectRequest>? config = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNull(bucketName, nameof(bucketName));
            Validator.RequireNotNull(objectKey, nameof(objectKey));

            if (!File.Exists(file))
                throw new FileNotFoundException("The file does not exist.", file);

            using (FileStream fs = File.OpenRead(file))
                return await client.PutObjectAsync(bucketName, objectKey, fs, config, token).ConfigureAwait(false);
        }

        /// <summary>List all objects in a bucket</summary>
        /// <param name="client">The BucketClient</param>
        /// <param name="bucketName">The name of the bucket you want to list objects in.</param>
        /// <param name="getOwnerInfo">Set to true if you want to get object owner information as well.</param>
        /// <param name="config">Delegate to configure the ListObjectsRequest before sending it</param>
        /// <param name="token">A cancellation token</param>
        public static async IAsyncEnumerable<S3Object> ListAllObjectsAsync(this IObjectClient client, string bucketName, bool getOwnerInfo = false, Action<ListObjectsRequest>? config = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(client, nameof(client));
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            string? continuationToken = null;
            ListObjectsResponse response;

            do
            {
                if (token.IsCancellationRequested)
                    break;

                string? cToken = continuationToken;
                response = await client.ListObjectsAsync(bucketName, req =>
                {
                    req.ContinuationToken = cToken;

                    if (getOwnerInfo)
                        req.FetchOwner = true;

                    config?.Invoke(req);
                }, token).ConfigureAwait(false);

                if (!response.IsSuccess)
                    throw new S3RequestException(response, "Request failed");

                foreach (S3Object responseObject in response.Objects)
                {
                    yield return responseObject;
                }

                continuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);
        }
    }
}
#endif