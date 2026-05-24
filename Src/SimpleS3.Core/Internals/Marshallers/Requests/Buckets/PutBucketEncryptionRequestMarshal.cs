using Genbox.FastEnum;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketEncryptionRequestMarshal : IRequestMarshal<PutBucketEncryptionRequest>
{
    public Stream MarshalRequest(PutBucketEncryptionRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Encryption, string.Empty);

        FastXmlWriter writer = new FastXmlWriter(300);
        writer.WriteStartElement("ServerSideEncryptionConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");

        foreach (S3ServerSideEncryptionRule rule in request.Rules)
        {
            writer.WriteStartElement("Rule");

            if (rule.SseAlgorithm != SseAlgorithm.Unknown)
            {
                writer.WriteStartElement("ApplyServerSideEncryptionByDefault");
                writer.WriteElement("SSEAlgorithm", rule.SseAlgorithm.GetDisplayName());

                if (rule.KmsMasterKeyId != null)
                    writer.WriteElement("KMSMasterKeyID", rule.KmsMasterKeyId);

                writer.WriteEndElement("ApplyServerSideEncryptionByDefault");
            }

            if (rule.BlockedEncryptionTypes.Count > 0)
            {
                writer.WriteStartElement("BlockedEncryptionTypes");

                foreach (BlockedEncryptionType encryptionType in rule.BlockedEncryptionTypes)
                {
                    if (encryptionType != BlockedEncryptionType.Unknown)
                        writer.WriteElement("EncryptionType", encryptionType.GetDisplayName());
                }

                writer.WriteEndElement("BlockedEncryptionTypes");
            }

            if (rule.BucketKeyEnabled.HasValue)
                writer.WriteElement("BucketKeyEnabled", rule.BucketKeyEnabled.Value ? "true" : "false");

            writer.WriteEndElement("Rule");
        }

        writer.WriteEndElement("ServerSideEncryptionConfiguration");

        return new MemoryStream(writer.GetBytes());
    }
}
