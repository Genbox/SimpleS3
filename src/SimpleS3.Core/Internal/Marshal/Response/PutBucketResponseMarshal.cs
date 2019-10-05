using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class PutBucketResponseMarshal : IResponseMarshal<PutBucketRequest, PutBucketResponse>
    {
        public void MarshalResponse(PutBucketRequest request, PutBucketResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
        }
    }
}