using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketEncryptionResponseMarshal : IResponseMarshal<GetBucketEncryptionResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketEncryptionResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.MoveToContent();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Rule")
                response.Rules.Add(ParseRule(reader));
        }
    }

    private static S3ServerSideEncryptionRule ParseRule(XmlReader reader)
    {
        SseAlgorithm algorithm = SseAlgorithm.Unknown;
        string? kmsKeyId = null;
        bool? bucketKeyEnabled = null;
        List<BlockedEncryptionType> blockedTypes = new List<BlockedEncryptionType>();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "ApplyServerSideEncryptionByDefault":
                    ReadDefaults(reader, ref algorithm, ref kmsKeyId);
                    break;
                case "BlockedEncryptionTypes":
                    ReadBlockedTypes(reader, blockedTypes);
                    break;
                case "BucketKeyEnabled":
                    bucketKeyEnabled = ValueHelper.ParseBool(reader.ReadString());
                    break;
            }
        }

        S3ServerSideEncryptionRule rule = new S3ServerSideEncryptionRule(algorithm)
        {
            KmsMasterKeyId = kmsKeyId,
            BucketKeyEnabled = bucketKeyEnabled
        };

        foreach (BlockedEncryptionType type in blockedTypes)
            rule.BlockedEncryptionTypes.Add(type);

        return rule;
    }

    private static void ReadDefaults(XmlReader reader, ref SseAlgorithm algorithm, ref string? kmsKeyId)
    {
        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "SSEAlgorithm":
                    algorithm = Core.Enums.Enums.SseAlgorithm.Parse(reader.ReadString(), SseAlgorithmFormat.DisplayName);
                    break;
                case "KMSMasterKeyID":
                    kmsKeyId = reader.ReadString();
                    break;
            }
        }
    }

    private static void ReadBlockedTypes(XmlReader reader, List<BlockedEncryptionType> blockedTypes)
    {
        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "EncryptionType")
                blockedTypes.Add(Core.Enums.Enums.BlockedEncryptionType.Parse(reader.ReadString(), BlockedEncryptionTypeFormat.DisplayName));
        }
    }
}