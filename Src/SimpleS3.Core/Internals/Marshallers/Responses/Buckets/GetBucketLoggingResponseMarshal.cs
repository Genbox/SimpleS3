using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketLoggingResponseMarshal : IResponseMarshal<GetBucketLoggingResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketLoggingResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.MoveToContent();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "LoggingEnabled")
            {
                response.LoggingConfiguration = ParseS3Configuration(reader);
                break;
            }
        }
    }

    private static S3LoggingConfiguration ParseS3Configuration(XmlReader reader)
    {
        string? bucket = null;
        string? prefix = null;
        List<S3Grant> grants = [];
        S3LoggingObjectKeyFormat? keyFormat = null;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "TargetBucket":
                    bucket = reader.ReadString();
                    break;
                case "TargetPrefix":
                    prefix = reader.ReadString();
                    break;
                case "TargetGrants":
                    ReadGrants(reader, grants);
                    break;
                case "TargetObjectKeyFormat":
                    keyFormat = ReadObjectKeyFormat(reader);
                    break;
            }
        }

        S3LoggingConfiguration configuration = new S3LoggingConfiguration(bucket ?? string.Empty, prefix) { TargetObjectKeyFormat = keyFormat };
        foreach (S3Grant grant in grants)
            configuration.TargetGrants.Add(grant);
        return configuration;
    }

    private static void ReadGrants(XmlReader reader, List<S3Grant> grants)
    {
        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name != "Grant")
                continue;

            S3Grant grant = ParserHelper.ParseGrant(reader);
            grants.Add(grant);
        }
    }

    private static S3LoggingObjectKeyFormat? ReadObjectKeyFormat(XmlReader reader)
    {
        S3LoggingObjectKeyFormat? format = null;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "SimplePrefix":
                    format = S3LoggingObjectKeyFormat.SimplePrefix;
                    break;
                case "PartitionedPrefix":
                    format = ReadPartitionedPrefix(reader);
                    break;
            }
        }

        return format;
    }

    private static S3LoggingObjectKeyFormat? ReadPartitionedPrefix(XmlReader reader)
    {
        S3LoggingObjectKeyFormat? format = S3LoggingObjectKeyFormat.PartitionedPrefixEventTime;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "PartitionDateSource")
            {
                format = reader.ReadString() switch
                {
                    "DeliveryTime" => S3LoggingObjectKeyFormat.PartitionedPrefixDeliveryTime,
                    "EventTime" => S3LoggingObjectKeyFormat.PartitionedPrefixEventTime,
                    _ => null
                };
            }
        }

        return format;
    }
}