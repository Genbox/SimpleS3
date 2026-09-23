using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class PutObjectTaggingRequestMarshal : IRequestMarshal<PutObjectTaggingRequest>
{
    public Stream MarshalRequest(PutObjectTaggingRequest request, SimpleS3Config config)
    {
        TagBuilder tags = new TagBuilder(10);
        foreach (KeyValuePair<string, string> tag in request.Tags)
            tags.Add(tag.Key, tag.Value);

        request.SetQueryParameter(AmzParameters.Tagging, string.Empty);

        FastXmlWriter writer = new FastXmlWriter(512);
        writer.WriteStartElement("Tagging", XmlConstants.S3Namespace);
        writer.WriteStartElement("TagSet");

        foreach (KeyValuePair<string, string> tag in tags)
        {
            writer.WriteStartElement("Tag");
            writer.WriteElement("Key", tag.Key);
            writer.WriteElement("Value", tag.Value);
            writer.WriteEndElement("Tag");
        }

        writer.WriteEndElement("TagSet");
        writer.WriteEndElement("Tagging");
        return new MemoryStream(writer.GetBytes());
    }
}