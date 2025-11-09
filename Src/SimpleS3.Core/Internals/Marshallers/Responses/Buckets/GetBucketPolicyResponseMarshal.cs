using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketPolicyResponseMarshal : IResponseMarshal<GetBucketPolicyResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketPolicyResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using MemoryStream ms = new MemoryStream();
        responseStream.CopyTo(ms);
        ms.Position = 0;

        response.Policy = Encoding.UTF8.GetString(ms.ToArray());
    }
}