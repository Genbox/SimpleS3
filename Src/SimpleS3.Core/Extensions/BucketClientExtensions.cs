using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Extensions;

/// <summary>Provides the <c>BucketClientExtensions</c> API.</summary>
public static class BucketClientExtensions
{
    /// <summary>Removes the bucket's default retention rule while keeping Object Lock enabled. Existing object retention is unchanged.</summary>
    public static Task<PutObjectLockConfigurationResponse> ClearBucketDefaultRetentionAsync(this IBucketClient client, string bucketName, Action<PutObjectLockConfigurationRequest>? config = null, CancellationToken token = default) => client.PutObjectLockConfigurationAsync(bucketName, true, request =>
    {
        config?.Invoke(request);
        request.Enabled = true;
        request.LockMode = LockMode.Unknown;
        request.LockRetainDays = null;
        request.LockRetainYears = null;
    }, token);

    /// <summary>List all buckets</summary>
    public static async IAsyncEnumerable<S3Bucket> ListAllBucketsAsync(this IBucketClient client, Action<ListBucketsRequest>? config = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        ListBucketsResponse response = await client.ListBucketsAsync(config, token);

        if (!response.IsSuccess)
            throw new S3RequestException(response, "Request failed");

        token.ThrowIfCancellationRequested();

        foreach (S3Bucket bucket in response.Buckets)
        {
            token.ThrowIfCancellationRequested();

            yield return bucket;
        }
    }
}