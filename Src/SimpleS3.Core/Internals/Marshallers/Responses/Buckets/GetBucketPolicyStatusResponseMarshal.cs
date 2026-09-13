using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketPolicyStatusResponseMarshal : IResponseMarshal<GetBucketPolicyStatusResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketPolicyStatusResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.MoveToContent();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name != "IsPublic")
                continue;

            string value = reader.ReadString().Trim();
            response.IsPublic = value == "1" || ValueHelper.ParseBool(value);
            break;
        }
    }
}