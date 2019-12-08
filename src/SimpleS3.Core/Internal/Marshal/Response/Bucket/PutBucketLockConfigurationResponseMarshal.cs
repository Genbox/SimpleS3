using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Bucket
{
    [UsedImplicitly]
    internal class PutBucketLockConfigurationResponseMarshal : IResponseMarshal<PutBucketLockConfigurationRequest, PutBucketLockConfigurationResponse>
    {
        public void MarshalResponse(IS3Config config, PutBucketLockConfigurationRequest request, PutBucketLockConfigurationResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}