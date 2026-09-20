using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketOwnershipControlsResponseMarshal : IResponseMarshal<GetBucketOwnershipControlsResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketOwnershipControlsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);

        while (reader.Read())
        {
            if (reader.IsStartElement() && reader.Name == "ObjectOwnership")
                response.Rules.Add(new S3OwnershipControlsRule(Core.Enums.Enums.ObjectOwnership.Parse(reader.ReadElementContentAsString())));
        }
    }
}