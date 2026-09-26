using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Extensions;

/// <summary>Provides the <c>ObjectClientExtensions</c> API.</summary>
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
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        DeleteObjectResponse resp = await client.DeleteObjectAsync(bucketName, objectKey, req =>
        {
            req.VersionId = versionId;

            if (mfa != null)
                req.Mfa = mfa;
        }, token);

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
        Validator.RequireNotNull(bucketName);

        return client.DeleteObjectsAsync(bucketName, objectKeys.Select(x => new S3DeleteInfo(x)), config, token);
    }

    /// <summary> Delete all objects within a bucket </summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="prefix">A prefix for all the objects to delete</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static IAsyncEnumerable<S3DeleteError> DeleteAllObjectsAsync(this IObjectClient client, string bucketName, string prefix, CancellationToken token = default) => client.DeleteAllObjectsAsync(bucketName, req => req.Prefix = prefix, token);

    /// <summary>Delete all objects within the bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="configure">A delegate you can use to configure the list objects request before it is sent off</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static IAsyncEnumerable<S3DeleteError> DeleteAllObjectsAsync(this IObjectClient client, string bucketName, Action<ListObjectsRequest>? configure = null, CancellationToken token = default) => client.DeleteAllObjectsAsync(bucketName, configure, null, token);

    /// <summary>Deletes listed objects using separate callbacks for listing and each delete batch.</summary>
    /// <remarks>The delete callback can supply MFA, governance bypass, payer, and expected-owner settings. Quiet defaults to false before the callback runs.</remarks>
    public static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectsAsync(this IObjectClient client, string bucketName, Action<ListObjectsRequest>? configure, Action<DeleteObjectsRequest>? configureDelete, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        Task<ListObjectsResponse>? responseTask = null;
        HashSet<string> seenTokens = new HashSet<string>(StringComparer.Ordinal);
        CancellationTokenSource prefetchCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

        ObjectPool<S3DeleteInfo> pool = ObjectPool<S3DeleteInfo>.Shared;

        try
        {
            responseTask = client.ListObjectsAsync(bucketName, req =>
            {
                configure?.Invoke(req);
                EnsureNoDelimiter(req.Delimiter);
                if (!string.IsNullOrEmpty(req.ContinuationToken))
                    seenTokens.Add(req.ContinuationToken);
            }, prefetchCancellation.Token);

            ListObjectsResponse response;

            do
            {
                token.ThrowIfCancellationRequested();

                response = await RequirePrefetchTaskAsync(responseTask);
                responseTask = null;

                if (!response.IsSuccess)
                    throw new S3RequestException(response, $"Unable to list objects for deletion in bucket '{bucketName}");

                if (response.Objects.Count == 0 && !response.IsTruncated)
                    yield break;

                if (response.IsTruncated)
                {
                    string? cToken = response.NextContinuationToken;

                    // S3-007: Continuation tokens are opaque and must be replayed byte-for-byte.
                    if (string.IsNullOrEmpty(cToken) || !seenTokens.Add(cToken))
                        throw new InvalidOperationException("Object listing returned a missing or non-progressing continuation token.");

                    responseTask = client.ListObjectsAsync(bucketName, req =>
                    {
                        configure?.Invoke(req);
                        EnsureNoDelimiter(req.Delimiter);
                        req.ContinuationToken = cToken;
                    }, prefetchCancellation.Token);
                }

                if (response.Objects.Count > 0)
                {
                    List<S3DeleteInfo> delete = response.Objects.Select(x => pool.Rent(info => info.Initialize(x.ObjectKey))).ToList();

                    DeleteObjectsResponse multiDelResponse;

                    try
                    {
                        multiDelResponse = await client.DeleteObjectsAsync(bucketName, delete, req =>
                        {
                            req.Quiet = false;
                            configureDelete?.Invoke(req);
                        }, token);
                    }
                    finally
                    {
                        pool.Return(delete); // Return even after exception
                    }

                    if (!multiDelResponse.IsSuccess)
                        throw new S3RequestException(multiDelResponse, $"Unable to delete objects in bucket '{bucketName}'");

                    foreach (S3DeleteError error in multiDelResponse.Errors)
                    {
                        token.ThrowIfCancellationRequested();
                        yield return error;
                    }
                }
            } while (response.IsTruncated);
        }
        finally
        {
            prefetchCancellation.Cancel();
            await ObservePrefetchTaskAsync(responseTask);
            prefetchCancellation.Dispose();
        }
    }

    /// <summary>Delete all object versions within a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="prefix">A prefix for all the objects to delete</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(this IObjectClient client, string bucketName, string prefix, CancellationToken token = default) => client.DeleteAllObjectVersionsAsync(bucketName, req => req.Prefix = prefix, token);

    /// <summary>Delete all object versions within a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="configure">A delete to configure the list object versions request before it is sent off</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>This method yields all the errors that occurred while trying to delete the objects</returns>
    /// <exception cref="S3RequestException">If any of the requests fails this exception will be thrown</exception>
    public static IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(this IObjectClient client, string bucketName, Action<ListObjectVersionsRequest>? configure = null, CancellationToken token = default) => client.DeleteAllObjectVersionsAsync(bucketName, configure, null, token);

    /// <summary>Deletes listed objects using separate callbacks for listing and each delete batch.</summary>
    /// <remarks>The delete callback can supply MFA, governance bypass, payer, and expected-owner settings. Quiet defaults to false before the callback runs.</remarks>
    public static async IAsyncEnumerable<S3DeleteError> DeleteAllObjectVersionsAsync(this IObjectClient client, string bucketName, Action<ListObjectVersionsRequest>? configure, Action<DeleteObjectsRequest>? configureDelete, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        HashSet<(string Key, string? Version)> seenMarkers = [];
        Task<ListObjectVersionsResponse>? responseTask = null;
        CancellationTokenSource prefetchCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            responseTask = client.ListObjectVersionsAsync(bucketName, req =>
            {
                configure?.Invoke(req);
                EnsureNoDelimiter(req.Delimiter);
                if (!string.IsNullOrEmpty(req.KeyMarker))
                    seenMarkers.Add((req.KeyMarker, req.VersionIdMarker));
            }, prefetchCancellation.Token);

            ListObjectVersionsResponse response;

            do
            {
                token.ThrowIfCancellationRequested();

                response = await RequirePrefetchTaskAsync(responseTask);
                responseTask = null;

                if (!response.IsSuccess)
                    throw new S3RequestException(response, $"Unable to list objects for deletion in bucket '{bucketName}");

                if (response.Versions.Count + response.DeleteMarkers.Count == 0 && !response.IsTruncated)
                    yield break;

                if (response.IsTruncated)
                {
                    string? keyMarker = response.NextKeyMarker;
                    string? versionIdMarker = response.NextVersionIdMarker;

                    if (string.IsNullOrEmpty(keyMarker) || !seenMarkers.Add((keyMarker, versionIdMarker)))
                        throw new InvalidOperationException("Object version listing returned missing or non-progressing markers.");

                    responseTask = client.ListObjectVersionsAsync(bucketName, req =>
                    {
                        configure?.Invoke(req);
                        EnsureNoDelimiter(req.Delimiter);

                        // DPR-037: Version listings must advance both key-marker and version-id-marker.
                        req.KeyMarker = keyMarker;
                        req.VersionIdMarker = versionIdMarker;
                    }, prefetchCancellation.Token);
                }

                if (response.Versions.Count + response.DeleteMarkers.Count > 0)
                {
                    IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                               .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

                    DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucketName, delete, req =>
                    {
                        req.Quiet = false;
                        configureDelete?.Invoke(req);
                    }, token);

                    if (!multiDelResponse.IsSuccess)
                        throw new S3RequestException(multiDelResponse, $"Unable to delete objects in bucket '{bucketName}'");

                    foreach (S3DeleteError error in multiDelResponse.Errors)
                    {
                        token.ThrowIfCancellationRequested();
                        yield return error;
                    }
                }
            } while (response.IsTruncated);
        }
        finally
        {
            prefetchCancellation.Cancel();
            await ObservePrefetchTaskAsync(responseTask);
            prefetchCancellation.Dispose();
        }
    }

    /// <summary>Provides the <c>PutObjectDataAsync</c> API.</summary>
    public static async Task<PutObjectResponse> PutObjectDataAsync(this IObjectClient client, string bucketName, string objectKey, byte[] data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        using MemoryStream ms = new MemoryStream(data);
        return await client.PutObjectAsync(bucketName, objectKey, ms, config, token);
    }

    /// <summary>Provides the <c>PutObjectStringAsync</c> API.</summary>
    public static Task<PutObjectResponse> PutObjectStringAsync(this IObjectClient client, string bucketName, string objectKey, string content, Encoding? encoding = null, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        encoding ??= Constants.Utf8NoBom;

        return client.PutObjectDataAsync(bucketName, objectKey, encoding.GetBytes(content), config, token);
    }

    /// <summary>Provides the <c>PutObjectFileAsync</c> API.</summary>
    public static async Task<PutObjectResponse> PutObjectFileAsync(this IObjectClient client, string bucketName, string objectKey, string file, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNull(bucketName);
        Validator.RequireNotNull(objectKey);

        if (!File.Exists(file))
            throw new FileNotFoundException("The file does not exist.", file);

        using FileStream fs = File.OpenRead(file);
        return await client.PutObjectAsync(bucketName, objectKey, fs, config, token);
    }

    /// <summary>List all objects within a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The bucket</param>
    /// <param name="prefix">A prefix to match on all the objects</param>
    /// <param name="getOwnerInfo">Set to true if you need owner information on the response objects</param>
    /// <param name="token">A cancellation token</param>
    /// <returns>A list of objects</returns>
    public static IAsyncEnumerable<S3Object> ListAllObjectsAsync(this IObjectClient client, string bucketName, string prefix, bool getOwnerInfo = false, CancellationToken token = default) => client.ListAllObjectsAsync(bucketName, req =>
    {
        req.Prefix = prefix;
        req.FetchOwner = getOwnerInfo;
    }, token);

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

        Task<ListObjectsResponse>? responseTask = null;
        HashSet<string> seenTokens = new HashSet<string>(StringComparer.Ordinal);
        CancellationTokenSource prefetchCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            responseTask = client.ListObjectsAsync(bucketName, request =>
            {
                configure?.Invoke(request);
                if (!string.IsNullOrEmpty(request.ContinuationToken))
                    seenTokens.Add(request.ContinuationToken);
            }, prefetchCancellation.Token);

            ListObjectsResponse response;

            do
            {
                token.ThrowIfCancellationRequested();

                response = await RequirePrefetchTaskAsync(responseTask);
                responseTask = null;

                if (!response.IsSuccess)
                    throw new S3RequestException(response, $"Unable to list objects in bucket '{bucketName}");

                if (response.Objects.Count == 0 && !response.IsTruncated)
                    yield break;

                if (response.IsTruncated)
                {
                    string? cToken = response.NextContinuationToken;

                    // S3-007: Continuation tokens are opaque and must be replayed byte-for-byte.
                    if (string.IsNullOrEmpty(cToken) || !seenTokens.Add(cToken))
                        throw new InvalidOperationException("Object listing returned a missing or non-progressing continuation token.");

                    responseTask = client.ListObjectsAsync(bucketName, req =>
                    {
                        configure?.Invoke(req);
                        req.ContinuationToken = cToken;
                    }, prefetchCancellation.Token);
                }

                foreach (S3Object obj in response.Objects)
                {
                    token.ThrowIfCancellationRequested();
                    yield return obj;
                }
            } while (response.IsTruncated);
        }
        finally
        {
            prefetchCancellation.Cancel();
            await ObservePrefetchTaskAsync(responseTask);
            prefetchCancellation.Dispose();
        }
    }

    /// <summary>List all object versions in a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The name of the bucket you want to list objects in.</param>
    /// <param name="prefix">A prefix to use</param>
    /// <param name="token">A cancellation token</param>
    public static IAsyncEnumerable<S3Version> ListAllObjectVersionsAsync(this IObjectClient client, string bucketName, string prefix, CancellationToken token = default) => client.ListAllObjectVersionsAsync(bucketName, req => req.Prefix = prefix, token);

    /// <summary>List all object versions in a bucket</summary>
    /// <param name="client">The ObjectClient</param>
    /// <param name="bucketName">The name of the bucket you want to list objects in.</param>
    /// <param name="configure">Delegate to configure the <see cref="ListObjectVersionsRequest" /> before sending it</param>
    /// <param name="token">A cancellation token</param>
    public static async IAsyncEnumerable<S3Version> ListAllObjectVersionsAsync(this IObjectClient client, string bucketName, Action<ListObjectVersionsRequest>? configure = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        HashSet<(string Key, string? Version)> seenMarkers = [];
        Task<ListObjectVersionsResponse>? responseTask = null;
        CancellationTokenSource prefetchCancellation = CancellationTokenSource.CreateLinkedTokenSource(token);

        try
        {
            responseTask = client.ListObjectVersionsAsync(bucketName, req =>
            {
                configure?.Invoke(req);
                if (!string.IsNullOrEmpty(req.KeyMarker))
                    seenMarkers.Add((req.KeyMarker, req.VersionIdMarker));
            }, prefetchCancellation.Token);

            ListObjectVersionsResponse response;

            do
            {
                token.ThrowIfCancellationRequested();

                response = await RequirePrefetchTaskAsync(responseTask);
                responseTask = null;

                if (!response.IsSuccess)
                    throw new S3RequestException(response, $"Unable to list object versions in bucket '{bucketName}");

                if (response.Versions.Count + response.DeleteMarkers.Count == 0 && !response.IsTruncated)
                    yield break;

                if (response.IsTruncated)
                {
                    string? keyMarker = response.NextKeyMarker;
                    string? versionIdMarker = response.NextVersionIdMarker;

                    if (string.IsNullOrEmpty(keyMarker) || !seenMarkers.Add((keyMarker, versionIdMarker)))
                        throw new InvalidOperationException("Object version listing returned missing or non-progressing markers.");

                    responseTask = client.ListObjectVersionsAsync(bucketName, req =>
                    {
                        configure?.Invoke(req);

                        // DPR-037: Version listings must advance both key-marker and version-id-marker.
                        req.KeyMarker = keyMarker;
                        req.VersionIdMarker = versionIdMarker;
                    }, prefetchCancellation.Token);
                }

                foreach (S3Version s3Version in response.Versions)
                {
                    token.ThrowIfCancellationRequested();
                    yield return s3Version;
                }
            } while (response.IsTruncated);
        }
        finally
        {
            prefetchCancellation.Cancel();
            await ObservePrefetchTaskAsync(responseTask);
            prefetchCancellation.Dispose();
        }
    }

    /// <summary>List all delete markers in a bucket without changing the existing version enumeration API.</summary>
    public static async IAsyncEnumerable<S3DeleteMarker> ListAllDeleteMarkersAsync(this IObjectClient client, string bucketName, Action<ListObjectVersionsRequest>? configure = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(client);
        Validator.RequireNotNullOrEmpty(bucketName);

        string? keyMarker = null;
        string? versionIdMarker = null;
        bool firstPage = true;
        HashSet<(string Key, string? Version)> seenMarkers = [];

        while (true)
        {
            string? currentKeyMarker = keyMarker;
            string? currentVersionIdMarker = versionIdMarker;
            ListObjectVersionsResponse response = await client.ListObjectVersionsAsync(bucketName, req =>
            {
                configure?.Invoke(req);

                if (firstPage)
                {
                    currentKeyMarker = req.KeyMarker;
                    currentVersionIdMarker = req.VersionIdMarker;
                    if (!string.IsNullOrEmpty(currentKeyMarker))
                        seenMarkers.Add((currentKeyMarker, currentVersionIdMarker));
                }
                else
                {
                    req.KeyMarker = currentKeyMarker;
                    req.VersionIdMarker = currentVersionIdMarker;
                }
            }, token);
            firstPage = false;

            if (!response.IsSuccess)
                throw new S3RequestException(response, $"Unable to list object versions in bucket '{bucketName}");

            foreach (S3DeleteMarker marker in response.DeleteMarkers)
            {
                token.ThrowIfCancellationRequested();
                yield return marker;
            }

            if (!response.IsTruncated)
                yield break;

            keyMarker = response.NextKeyMarker;
            versionIdMarker = response.NextVersionIdMarker;
            if (string.IsNullOrEmpty(keyMarker) || !seenMarkers.Add((keyMarker, versionIdMarker)))
                throw new InvalidOperationException("Object version listing returned missing or non-progressing markers.");
        }
    }

    [SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "Cleanup intentionally observes the pre-started speculative prefetch task.")]
    private static async Task ObservePrefetchTaskAsync(Task? task)
    {
        if (task == null)
            return;

        try
        {
            await task;
        }
        catch
        {
            // The abandoned prefetch is canceled and observed so it cannot surface later as an unobserved exception.
        }
    }

    private static Task<T> RequirePrefetchTaskAsync<T>(Task<T>? task) => task ?? throw new InvalidOperationException("A paginated request did not schedule its next page.");

    private static void EnsureNoDelimiter(string? delimiter)
    {
        if (delimiter != null)
            throw new ArgumentException("Delete-all operations do not support Delimiter because grouped objects would be skipped.", nameof(delimiter));
    }
}