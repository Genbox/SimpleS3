using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class RestoreObjectResponseMarshal : IResponseMarshal<RestoreObjectRequest, RestoreObjectResponse>
    {
        public void MarshalResponse(IS3Config config, RestoreObjectRequest request, RestoreObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
            response.RestoreOutputPath = headers.GetHeader(AmzHeaders.XAmzRestoreOutputPath);
        }
    }
}