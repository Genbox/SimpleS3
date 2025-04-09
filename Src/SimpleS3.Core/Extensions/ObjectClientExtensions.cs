using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Extensions;

public static class ObjectClientExtensions
{
    /// <summary>Delete a single object</summary>
    /// <param name="client">The ObjectClient to use</param>
    /// <param name="bucketName">The bucket the object resides in</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="versionId">The version of the object</param>
    /// <param name="mfa">If MFA is enabled on the object, give the MFA information here</param>
    /// <param name="token">A cancellation token to cancel the request</param>
    public static async Task<DeleteObjectResponse> DeleteObjectAsync(this IObjectClient client, string bucketName, string objectKey, string? versionId = null, MfaAuthenticationBuilder? mfa = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        DeleteObjectResponse resp = await client.DeleteObjectAsync(bucketName, objectKey, req =>
        {
            req.VersionId = versionId;

            if (mfa != null)
                req.Mfa = mfa;
        }, token).ConfigureAwait(false);

        return resp;
    }

    /// <summary>Delete multiple objects</summary>
    /// <param name="client"></param>
    /// <param name="bucketName">The bucket the object resides in</param>
    /// <param name="objectKeys">A list of keys you want to delete</param>
    /// <param name="config">Used this to configure the request before it is sent</param>
    /// <param name="token">A cancellation token to cancel the request</param>
    public static Task<DeleteObjectsResponse> DeleteObjectsAsync(this IObjectClient client, string bucketName, IEnumerable<string> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKeys);

        return client.DeleteObjectsAsync(bucketName, objectKeys.Select(x => new S3DeleteInfo(x)), config, token);
    }

    /// <summary> Delete all objects within a bucket </summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="prefix">A prefix for all the objects to delete</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static IAsyncEnumerable<S3DeleteError> DeleteAllObjectsAsync(this IObjectClient client, string bucketName, string prefix, CancellationToken token = default)
    {
        return DeleteAllObjectsAsync(client, bucketName, req => req.Prefix = prefix, token);
    }

    /// <summary>Delete all objects within the bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="configure">A delegate you can use to configure the list objects request before it is sent off</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectsAsync(this IObjectClient client, string bucketName, Action<ListObjectsRequest>? configure = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        ListObjectsResponse response;
        Task<ListObjectsResponse> responseTask = client.ListObjectsAsync(bucketName, configure, token);

        ObjectPool<S3DeleteInfo> pool = ObjectPool<S3DeleteInfo>.Shared;

        do
        {
            if (token.IsCancellationRequested)
                yield break;

            response = await responseTask.ConfigureAwait(false);

            if (!response.IsSuccess)
                throw new S3RequestException(response, $"Unable to list objects for deletion in bucket '{bucketName}");

            if (response.Objects.Count == 0)
                yield break;

            if (response.IsTruncated)
            {
                string cToken = response.NextContinuationToken;

                if (response.EncodingType == EncodingType.Url)
                    cToken = HttpUtility.UrlDecode(cToken);

                responseTask = client.ListObjectsAsync(bucketName, req =>
                {
                    req.ContinuationToken = cToken;
                    configure?.Invoke(req);
                }, token);
            }

            List<S3DeleteInfo> delete = response.Objects.Select(x => pool.Rent(info => info.Initialize(x.ObjectKey))).ToList();

            DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucketName, delete, req => req.Quiet = false, token).ConfigureAwait(false);

            pool.Return(delete);

            if (!multiDelResponse.IsSuccess)
                throw new S3RequestException(response, $"Unable to delete objects in bucket '{bucketName}");

            foreach (S3DeleteError error in multiDelResponse.Errors)
                yield return error;
        } while (response.IsTruncated);
    }

    /// <summary>Delete all object versions within a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="prefix">A prefix for all the objects to delete</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(this IObjectClient client, string bucketName, string prefix, CancellationToken token = default)
    {
        return DeleteAllObjectVersionsAsync(client, bucketName, req => req.Prefix = prefix, token);
    }

    /// <summary>Delete all object versions within a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="configure">A delete to configure the list object versions request before it is sent off</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(this IObjectClient client, string bucketName, Action<ListObjectVersionsRequest>? configure = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        ListObjectVersionsResponse response;
        Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucketName, configure, token);

        do
        {
            if (token.IsCancellationRequested)
                break;

            response = await responseTask.ConfigureAwait(false);

            if (!response.IsSuccess)
                throw new S3RequestException(response, $"Unable to list objects for deletion in bucket '{bucketName}");

            if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                yield break;

            if (response.IsTruncated)
            {
                string keyMarker = response.NextKeyMarker;

                if (response.EncodingType == EncodingType.Url)
                    keyMarker = HttpUtility.UrlDecode(keyMarker);

                responseTask = client.ListObjectVersionsAsync(bucketName, req =>
                {
                    req.KeyMarker = keyMarker;
                    configure?.Invoke(req);
                }, token);
            }

            IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                       .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

            DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucketName, delete, req => req.Quiet = false, token).ConfigureAwait(false);

            if (!multiDelResponse.IsSuccess)
                throw new S3RequestException(response, $"Unable to delete objects in bucket '{bucketName}");

            foreach (S3DeleteError error in multiDelResponse.Errors)
                yield return error;
        } while (response.IsTruncated);
    }

    public static async Task<PutObjectResponse> PutObjectDataAsync(this IObjectClient client, string bucketName, string objectKey, byte[] data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        using MemoryStream ms = new MemoryStream(data);
        return await client.PutObjectAsync(bucketName, objectKey, ms, config, token).ConfigureAwait(false);
    }

    public static Task<PutObjectResponse> PutObjectStringAsync(this IObjectClient client, string bucketName, string objectKey, string content, Encoding? encoding = null, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        encoding ??= Constants.Utf8NoBom;

        return client.PutObjectDataAsync(bucketName, objectKey, encoding.GetBytes(content), config, token);
    }

    public static async Task<PutObjectResponse> PutObjectFileAsync(this IObjectClient client, string bucketName, string objectKey, string file, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        if (!File.Exists(file))
            throw new FileNotFoundException("The file does not exist.", file);

        using FileStream fs = File.OpenRead(file);
        return await client.PutObjectAsync(bucketName, objectKey, fs, config, token).ConfigureAwait(false);
    }

    /// <summary>List all objects within a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="prefix">A prefix to match on all the objects</param>
    /// <param name="getOwnerInfo">Set to true if you need owner information on the response objects</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>A list of objects</returns>
    public static IAsyncEnumerable<S3Object> ListAllObjectsAsync(this IObjectClient client, string bucketName, string prefix, bool getOwnerInfo = false, CancellationToken token = default)
    {
        return ListAllObjectsAsync(client, bucketName, req =>
        {
            req.Prefix = prefix;
            req.FetchOwner = getOwnerInfo;
        }, token);
    }

    /// <summary>List all objects within a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The name of the bucket you want to list objects in.</param>
    /// <param name="configure">A delegate to configure the ListObjectsRequest before sending it</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>A list of objects within the bucket</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static async IAsyncEnumerable<S3Object> ListAllObjectsAsync(this IObjectClient client, string bucketName, Action<ListObjectsRequest>? configure = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        ListObjectsResponse response;
        Task<ListObjectsResponse> responseTask = client.ListObjectsAsync(bucketName, configure, token);

        do
        {
            if (token.IsCancellationRequested)
                break;

            response = await responseTask.ConfigureAwait(false);

            if (!response.IsSuccess)
                throw new S3RequestException(response, $"Unable to list objects in bucket '{bucketName}");

            if (response.Objects.Count == 0)
                yield break;

            if (response.IsTruncated)
            {
                string cToken = response.NextContinuationToken;

                if (response.EncodingType == EncodingType.Url)
                    cToken = HttpUtility.UrlDecode(cToken);

                responseTask = client.ListObjectsAsync(bucketName, req =>
                {
                    req.ContinuationToken = cToken;
                    configure?.Invoke(req);
                }, token);
            }

            foreach (S3Object obj in response.Objects)
                yield return obj;
        } while (response.IsTruncated);
    }

    /// <summary>List all object versions in a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The name of the bucket you want to list objects in.</param>
    /// <param name="prefix">A prefix to use</param>
    /// <param name="token">A cancellation token</param>
    public static IAsyncEnumerable<S3Version> ListAllObjectVersionsAsync(this IObjectClient client, string bucketName, string prefix, CancellationToken token = default)
    {
        return ListAllObjectVersionsAsync(client, bucketName, req => req.Prefix = prefix, token);
    }

    /// <summary>List all object versions in a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The name of the bucket you want to list objects in.</param>
    /// <param name="configure">Delegate to configure the <see cref="ListObjectVersionsRequest" /> before sending it</param>
    /// <param name="token">A cancellation token</param>
    public static async IAsyncEnumerable<S3Version> ListAllObjectVersionsAsync(this IObjectClient client, string bucketName, Action<ListObjectVersionsRequest>? configure = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        ListObjectVersionsResponse response;
        Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucketName, configure, token);

        do
        {
            if (token.IsCancellationRequested)
                break;

            response = await responseTask.ConfigureAwait(false);

            if (!response.IsSuccess)
                throw new S3RequestException(response, $"Unable to list object versions in bucket '{bucketName}");

            if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                yield break;

            if (response.IsTruncated)
            {
                string keyMarker = response.NextKeyMarker;

                if (response.EncodingType == EncodingType.Url)
                    keyMarker = HttpUtility.UrlDecode(keyMarker);

                responseTask = client.ListObjectVersionsAsync(bucketName, req =>
                {
                    req.KeyMarker = keyMarker;
                    configure?.Invoke(req);
                }, token);
            }

            foreach (S3Version s3Version in response.Versions)
                yield return s3Version;
        } while (response.IsTruncated);
    }
}