using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    [UsedImplicitly]
    internal class PutBucketAccelerateConfigurationResponseMarshal : IResponseMarshal<PutBucketAccelerateConfigurationResponse>
    {
        public void MarshalResponse(IConfig config, PutBucketAccelerateConfigurationResponse response, IDictionary<string, string> headers, Stream responseStream) { }
    }
}