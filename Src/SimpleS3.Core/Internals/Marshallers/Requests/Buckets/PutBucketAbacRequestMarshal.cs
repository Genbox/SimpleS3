using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketAbacRequestMarshal : IRequestMarshal<PutBucketAbacRequest>
{
    public Stream MarshalRequest(PutBucketAbacRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("abac", string.Empty);
        FastXmlWriter writer = new FastXmlWriter(128);
        writer.WriteStartElement("AbacStatus", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("Status", request.Enabled ? "Enabled" : "Disabled");
        writer.WriteEndElement("AbacStatus");
        return new MemoryStream(writer.GetBytes());
    }
}