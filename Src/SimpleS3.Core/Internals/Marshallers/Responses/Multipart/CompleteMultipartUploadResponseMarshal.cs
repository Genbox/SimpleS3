using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;

internal sealed class CompleteMultipartUploadResponseMarshal : IResponseMarshal<CompleteMultipartUploadResponse>
{
    public void MarshalResponse(SimpleS3Config config, CompleteMultipartUploadResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSse, out string? sseHeader))
            response.SseAlgorithm = Core.Enums.Enums.SseAlgorithm.Parse(sseHeader, SseAlgorithmFormat.DisplayName);

        response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSseCustomerAlgorithm, out string? sseAlgorithm))
            response.SseCustomerAlgorithm = Core.Enums.Enums.SseCustomerAlgorithm.Parse(sseAlgorithm, SseCustomerAlgorithmFormat.DisplayName);

        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        if (ParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
        {
            response.LifeCycleExpiresOn = data.expiresOn;
            response.LifeCycleRuleId = data.ruleId;
        }

        using XmlTextReader xmlReader = new XmlTextReader(responseStream);
        xmlReader.ReadToDescendant("CompleteMultipartUploadResult");

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            switch (name)
            {
                case "Location":
                    response.Location = xmlReader.ReadString();
                    break;
                case "Bucket":
                    response.BucketName = xmlReader.ReadString();
                    break;
                case "Key":
                    response.ObjectKey = xmlReader.ReadString();
                    break;
                case "ETag":
                    response.ETag = xmlReader.ReadString();
                    break;
                case "ChecksumType":
                    response.ChecksumType = Core.Enums.Enums.ChecksumType.Parse(xmlReader.ReadString(), ChecksumTypeFormat.DisplayName);
                    break;
                case "ChecksumCRC32":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Crc32;
                    response.Checksum = ParseHash(xmlReader.ReadString());
                    break;
                case "ChecksumCRC32C":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Crc32C;
                    response.Checksum = ParseHash(xmlReader.ReadString());
                    break;
                case "ChecksumCRC64NVME":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Crc64Nvme;
                    response.Checksum = ParseHash(xmlReader.ReadString());
                    break;
                case "ChecksumSHA1":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
                    response.Checksum = ParseHash(xmlReader.ReadString());
                    break;
                case "ChecksumSHA256":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Sha256;
                    response.Checksum = ParseHash(xmlReader.ReadString());
                    break;
            }
        }
    }

    private static byte[] ParseHash(string input)
    {
        int idx = input.IndexOf('-');

        if (idx >= 0)
            input = input.Substring(0, idx);

        return Convert.FromBase64String(input);
    }
}