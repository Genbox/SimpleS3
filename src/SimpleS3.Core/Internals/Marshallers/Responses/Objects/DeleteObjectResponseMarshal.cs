using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    internal class DeleteObjectResponseMarshal : IResponseMarshal<DeleteObjectResponse>
    {
        public void MarshalResponse(SimpleS3Config config, DeleteObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.IsDeleteMarker = headers.GetHeaderBool(AmzHeaders.XAmzDeleteMarker);
            response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}