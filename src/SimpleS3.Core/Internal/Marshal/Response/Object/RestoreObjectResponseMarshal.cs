using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Object
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