using System.IO;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class ListObjectsRequestMarshal : IRequestMarshal<ListObjectsRequest>
    {
        public Stream MarshalRequest(ListObjectsRequest request)
        {
            request.AddQueryParameter(BucketParameters.Delimiter, request.Delimiter);
            request.AddQueryParameter(BucketParameters.EncodingType, request.EncodingType);
            request.AddQueryParameter(BucketParameters.MaxKeys, request.MaxKeys);
            request.AddQueryParameter(BucketParameters.Prefix, request.Prefix);
            request.AddQueryParameter(BucketParameters.ContinuationToken, request.ContinuationToken);
            request.AddQueryParameter(BucketParameters.FetchOwner, request.FetchOwner);
            request.AddQueryParameter(BucketParameters.StartAfter, request.StartAfter);
            request.AddQueryParameter(BucketParameters.ListType, 2);
            return null;
        }
    }
}