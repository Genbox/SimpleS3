using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Errors;
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
        response.BucketKeyEnabled = headers.GetHeaderNullableBool(AmzHeaders.XAmzSseBucketKeyEnabled);

        if (headers.TryGetHeader(AmzHeaders.XAmzSseCustomerAlgorithm, out string? sseAlgorithm))
            response.SseCustomerAlgorithm = Core.Enums.Enums.SseCustomerAlgorithm.Parse(sseAlgorithm, SseCustomerAlgorithmFormat.DisplayName);

        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        if (ParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
        {
            response.LifeCycleExpiresOn = data.expiresOn;
            response.LifeCycleRuleId = data.ruleId;
        }

        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);

        // S3-002: CompleteMultipartUpload can report an Error document with HTTP 200.
        if (xmlReader.MoveToContent() == XmlNodeType.Element && xmlReader.LocalName == "Error")
        {
            response.IsSuccess = false;
            response.Error = ErrorHandler.Create(xmlReader);
            return;
        }

        if (xmlReader.LocalName != "CompleteMultipartUploadResult" && !XmlHelper.ReadToDescendant(xmlReader, "CompleteMultipartUploadResult"))
            return;

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            ChecksumAlgorithm algorithm = ChecksumHelper.GetXmlAlgorithm(name);

            if (algorithm != ChecksumAlgorithm.Unknown)
            {
                response.ChecksumAlgorithm = algorithm;
                response.Checksum = ChecksumHelper.ParseHash(xmlReader.ReadString());
                continue;
            }

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
            }
        }
    }
}