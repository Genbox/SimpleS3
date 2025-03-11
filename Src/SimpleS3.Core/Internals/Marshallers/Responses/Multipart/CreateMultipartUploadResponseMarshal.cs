using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;

internal sealed class CreateMultipartUploadResponseMarshal : IResponseMarshal<CreateMultipartUploadResponse>
{
    public void MarshalResponse(SimpleS3Config config, CreateMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Rfc1123);
        response.AbortRuleId = headers.GetOptionalValue(AmzHeaders.XAmzAbortRuleId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSse, out string? sseHeader))
            response.SseAlgorithm = Core.Enums.Enums.SseAlgorithm.Parse(sseHeader, SseAlgorithmFormat.DisplayName);

        response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSseCustomerAlgorithm, out string? sseAlgorithm))
            response.SseCustomerAlgorithm = Core.Enums.Enums.SseCustomerAlgorithm.Parse(sseAlgorithm, SseCustomerAlgorithmFormat.DisplayName);

        response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
        response.SseContext = headers.GetOptionalValue(AmzHeaders.XAmzSseContext);
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        if (headers.TryGetHeader(AmzHeaders.XAmzChecksumType, out string? checksumType))
            response.ChecksumType = Core.Enums.Enums.ChecksumType.Parse(checksumType, ChecksumTypeFormat.DisplayName);

        if (headers.TryGetHeader(AmzHeaders.XAmzChecksumAlgorithm, out string? checksumAlgorithm))
            response.ChecksumAlgorithm = Core.Enums.Enums.ChecksumAlgorithm.Parse(checksumAlgorithm, ChecksumAlgorithmFormat.DisplayName);

        using XmlTextReader xmlReader = new XmlTextReader(responseStream);
        xmlReader.ReadToDescendant("InitiateMultipartUploadResult");

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            switch (name)
            {
                case "Bucket":
                    response.BucketName = xmlReader.ReadString();
                    break;
                case "Key":
                    response.ObjectKey = xmlReader.ReadString();
                    break;
                case "UploadId":
                    response.UploadId = xmlReader.ReadString();
                    break;
            }
        }
    }
}