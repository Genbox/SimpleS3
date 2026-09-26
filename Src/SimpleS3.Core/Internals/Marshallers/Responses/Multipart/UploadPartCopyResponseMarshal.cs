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

internal sealed class UploadPartCopyResponseMarshal : IResponseMarshal<UploadPartCopyResponse>
{
    public void MarshalResponse(SimpleS3Config config, UploadPartCopyResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        if (headers.TryGetHeader(AmzHeaders.XAmzSse, out string? sseHeader))
            response.SseAlgorithm = Core.Enums.Enums.SseAlgorithm.Parse(sseHeader, SseAlgorithmFormat.DisplayName);

        response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);
        response.BucketKeyEnabled = headers.GetHeaderNullableBool(AmzHeaders.XAmzSseBucketKeyEnabled);
        response.CopySourceVersionId = headers.GetOptionalValue(AmzHeaders.XAmzCopySourceVersionId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSseCustomerAlgorithm, out string? sseAlgorithm))
            response.SseCustomerAlgorithm = Core.Enums.Enums.SseCustomerAlgorithm.Parse(sseAlgorithm, SseCustomerAlgorithmFormat.DisplayName);

        response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);

        (response.ChecksumAlgorithm, response.Checksum) = ChecksumHelper.ReadHeaders(headers);

        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        XmlHelper.ReadToDescendant(reader, "CopyPartResult");

        foreach (string name in XmlHelper.ReadElements(reader))
        {
            ChecksumAlgorithm algorithm = ChecksumHelper.GetXmlAlgorithm(name);

            if (algorithm != ChecksumAlgorithm.Unknown)
            {
                response.ChecksumAlgorithm = algorithm;
                response.Checksum = ChecksumHelper.ParseHash(reader.ReadString());
                continue;
            }

            switch (name)
            {
                case "ETag":
                    response.ETag = reader.ReadString();
                    break;
                case "LastModified":
                    response.LastModified = ValueHelper.ParseDate(reader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
            }
        }
    }
}