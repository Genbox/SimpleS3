using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal class RestoreObjectResponseMarshal : IResponseMarshal<RestoreObjectResponse>
{
    public void MarshalResponse(SimpleS3Config config, RestoreObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        response.RestoreOutputPath = headers.GetOptionalValue(AmzHeaders.XAmzRestoreOutputPath);
    }
}