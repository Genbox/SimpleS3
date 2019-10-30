using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class DeleteObjectResponseMarshal : IResponseMarshal<DeleteObjectRequest, DeleteObjectResponse>
    {
        public void MarshalResponse(DeleteObjectRequest request, DeleteObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.IsDeleteMarker = headers.GetHeaderBool(AmzHeaders.XAmzDeleteMarker);
            response.VersionId = headers.GetHeader(AmzHeaders.XAmzVersionId);
        }
    }
}