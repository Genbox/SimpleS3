using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Object
{
    [UsedImplicitly]
    internal class PutObjectLegalHoldResponseMarshal : IResponseMarshal<PutObjectLegalHoldRequest, PutObjectLegalHoldResponse>
    {
        public void MarshalResponse(IS3Config config, PutObjectLegalHoldRequest request, PutObjectLegalHoldResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}