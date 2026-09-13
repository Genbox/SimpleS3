using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketVersioningRequestMarshal : IRequestMarshal<PutBucketVersioningRequest>
{
    public Stream MarshalRequest(PutBucketVersioningRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Versioning, string.Empty);

        FastXmlWriter writer = new FastXmlWriter(128);
        writer.WriteStartElement("VersioningConfiguration", XmlConstants.S3Namespace);
        writer.WriteElement("Status", request.Status ? "Enabled" : "Suspended");

        if (request.MfaDelete.HasValue)
            writer.WriteElement("MfaDelete", request.MfaDelete.Value ? "Enabled" : "Disabled");

        writer.WriteEndElement("VersioningConfiguration");

        return new MemoryStream(writer.GetBytes());
    }
}