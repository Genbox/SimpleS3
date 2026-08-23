using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetPublicAccessBlockResponseMarshal : IResponseMarshal<GetPublicAccessBlockResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetPublicAccessBlockResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);

        while (reader.Read())
        {
            if (!reader.IsStartElement())
                continue;

            switch (reader.Name)
            {
                case "BlockPublicAcls":
                    response.BlockPublicAcls = ValueHelper.ParseBool(reader.ReadElementContentAsString());
                    break;
                case "IgnorePublicAcls":
                    response.IgnorePublicAcls = ValueHelper.ParseBool(reader.ReadElementContentAsString());
                    break;
                case "BlockPublicPolicy":
                    response.BlockPublicPolicy = ValueHelper.ParseBool(reader.ReadElementContentAsString());
                    break;
                case "RestrictPublicBuckets":
                    response.RestrictPublicBuckets = ValueHelper.ParseBool(reader.ReadElementContentAsString());
                    break;
            }
        }
    }
}