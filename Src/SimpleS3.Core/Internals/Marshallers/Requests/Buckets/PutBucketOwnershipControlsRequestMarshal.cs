using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketOwnershipControlsRequestMarshal : IRequestMarshal<PutBucketOwnershipControlsRequest>
{
    public Stream MarshalRequest(PutBucketOwnershipControlsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.OwnershipControls, string.Empty);
        FastXmlWriter writer = new FastXmlWriter(256);
        writer.WriteStartElement("OwnershipControls", XmlConstants.S3Namespace);

        foreach (S3OwnershipControlsRule rule in request.Rules)
        {
            writer.WriteStartElement("Rule");
            if (rule.ObjectOwnership != ObjectOwnership.Unknown)
                writer.WriteElement("ObjectOwnership", rule.ObjectOwnership.GetString());
            writer.WriteEndElement("Rule");
        }

        writer.WriteEndElement("OwnershipControls");
        return new MemoryStream(writer.GetBytes());
    }
}