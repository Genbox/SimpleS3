using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    internal class PutBucketLockConfigurationResponseMarshal : IResponseMarshal<PutBucketLockConfigurationResponse>
    {
        public void MarshalResponse(Config config, PutBucketLockConfigurationResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}