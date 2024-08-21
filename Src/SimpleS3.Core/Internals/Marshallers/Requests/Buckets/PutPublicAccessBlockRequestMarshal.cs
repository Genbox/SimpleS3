using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutPublicAccessBlockRequestMarshal : IRequestMarshal<PutPublicAccessBlockRequest>
{
    public Stream MarshalRequest(PutPublicAccessBlockRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("publicAccessBlock", string.Empty);

        FastXmlWriter xml = new FastXmlWriter(400);
        xml.WriteStartElement("PublicAccessBlockConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        xml.WriteElement("BlockPublicAcls", request.BlockPublicAcls ? "true" : "false");
        xml.WriteElement("BlockPublicPolicy", request.BlockPublicPolicy ? "true" : "false");
        xml.WriteElement("IgnorePublicAcls", request.IgnorePublicAcls ? "true" : "false");
        xml.WriteElement("RestrictPublicBuckets", request.RestrictPublicBuckets ? "true" : "false");
        xml.WriteEndElement("PublicAccessBlockConfiguration");

        return new MemoryStream(xml.GetBytes());
    }
}