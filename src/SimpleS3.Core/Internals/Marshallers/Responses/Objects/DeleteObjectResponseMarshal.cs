using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class DeleteObjectResponseMarshal : IResponseMarshal<DeleteObjectResponse>
    {
        public void MarshalResponse(IConfig config, DeleteObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.IsDeleteMarker = headers.GetHeaderBool(AmzHeaders.XAmzDeleteMarker);
            response.VersionId = headers.GetHeader(AmzHeaders.XAmzVersionId);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}