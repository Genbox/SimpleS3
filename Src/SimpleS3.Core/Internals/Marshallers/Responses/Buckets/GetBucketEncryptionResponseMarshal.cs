using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketEncryptionResponseMarshal : IResponseMarshal<GetBucketEncryptionResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketEncryptionResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);

        while (reader.ReadToFollowing("Rule"))
        {
            SseAlgorithm algorithm = SseAlgorithm.Unknown;
            string? kmsKeyId = null;
            bool? bucketKeyEnabled = null;
            List<BlockedEncryptionType> blockedTypes = new List<BlockedEncryptionType>();

            using XmlReader rule = reader.ReadSubtree();
            while (rule.Read())
            {
                if (!rule.IsStartElement())
                    continue;

                switch (rule.Name)
                {
                    case "SSEAlgorithm":
                        algorithm = Core.Enums.Enums.SseAlgorithm.Parse(rule.ReadElementContentAsString(), SseAlgorithmFormat.DisplayName);
                        break;
                    case "KMSMasterKeyID":
                        kmsKeyId = rule.ReadElementContentAsString();
                        break;
                    case "BucketKeyEnabled":
                        bucketKeyEnabled = ValueHelper.ParseBool(rule.ReadElementContentAsString());
                        break;
                    case "EncryptionType":
                        blockedTypes.Add(Core.Enums.Enums.BlockedEncryptionType.Parse(rule.ReadElementContentAsString(), BlockedEncryptionTypeFormat.DisplayName));
                        break;
                }
            }

            S3ServerSideEncryptionRule encryptionRule = new S3ServerSideEncryptionRule(algorithm)
            {
                KmsMasterKeyId = kmsKeyId,
                BucketKeyEnabled = bucketKeyEnabled
            };

            foreach (BlockedEncryptionType blockedType in blockedTypes)
                encryptionRule.BlockedEncryptionTypes.Add(blockedType);

            response.Rules.Add(encryptionRule);
        }
    }
}