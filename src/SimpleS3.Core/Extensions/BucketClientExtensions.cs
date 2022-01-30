#if COMMERCIAL
using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Extensions;

public static class BucketClientExtensions
{
    /// <summary>List all buckets</summary>
    public static async IAsyncEnumerable<S3Bucket> ListAllBucketsAsync(this IBucketClient client, Action<ListBucketsRequest>? config = null, [EnumeratorCancellation] CancellationToken token = default)
    {
        ListBucketsResponse response = await client.ListBucketsAsync(config, token).ConfigureAwait(false);

        if (!response.IsSuccess)
            throw new S3RequestException(response, "Request failed");

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
#endif