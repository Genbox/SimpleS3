using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class PutBucketTaggingRequestMarshal : IRequestMarshal<PutBucketTaggingRequest>
    {
        public Stream? MarshalRequest(PutBucketTaggingRequest request, IConfig config)
        {
            request.SetQueryParameter(AmzParameters.Tagging, string.Empty);

            if (!request.Tags.HasData())
                return null;

            FastXmlWriter writer = new FastXmlWriter(200);
            writer.WriteStartElement("Tagging", "http://s3.amazonaws.com/doc/2006-03-01/");
            writer.WriteStartElement("TagSet");

            foreach (KeyValuePair<string, string> tag in request.Tags)
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
}