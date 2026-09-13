using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketAbacResponseMarshal : IResponseMarshal<GetBucketAbacResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketAbacResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.MoveToContent();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Status")
            {
                response.Enabled = reader.ReadString() == "Enabled";
                return;
            }
        }
    }
}