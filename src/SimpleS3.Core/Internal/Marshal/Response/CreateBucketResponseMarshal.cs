using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class CreateBucketResponseMarshal : IResponseMarshal<CreateBucketRequest, CreateBucketResponse>
    {
        public void MarshalResponse(CreateBucketRequest request, CreateBucketResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
        }
    }
}