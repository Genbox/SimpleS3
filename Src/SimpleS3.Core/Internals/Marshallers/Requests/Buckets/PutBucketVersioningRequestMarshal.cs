using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal class PutBucketVersioningRequestMarshal : IRequestMarshal<PutBucketVersioningRequest>
{
    public Stream? MarshalRequest(PutBucketVersioningRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Versioning, string.Empty);

        FastXmlWriter writer = new FastXmlWriter(128);
        writer.WriteStartElement("VersioningConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("Status", request.Status ? "Enabled" : "Suspended");

        string? mfa = request.Mfa.Build();

        if (mfa != null)
            writer.WriteElement("MfaDelete", mfa);

        writer.WriteEndElement("VersioningConfiguration");

        return new MemoryStream(writer.GetBytes());
    }
}