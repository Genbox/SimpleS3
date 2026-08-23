using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class GetObjectTaggingResponseMarshal : IResponseMarshal<GetObjectTaggingResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetObjectTaggingResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.ReadToDescendant("TagSet");

        while (reader.Read())
        {
            if (!reader.IsStartElement() || reader.Name != "Tag")
                continue;

            reader.Read();
            string key = reader.ReadElementContentAsString();
            string value = reader.ReadElementContentAsString();
            response.Tags.Add(key, value);
        }
    }
}