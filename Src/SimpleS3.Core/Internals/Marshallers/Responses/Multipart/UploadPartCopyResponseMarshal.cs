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

        string? checksum = null;
        if (headers.TryGetHeader(AmzHeaders.XAmzChecksumCrc32, out checksum))
            response.ChecksumAlgorithm = ChecksumAlgorithm.Crc32;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumCrc32C, out checksum))
            response.ChecksumAlgorithm = ChecksumAlgorithm.Crc32C;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumCrc64Nvme, out checksum))
            response.ChecksumAlgorithm = ChecksumAlgorithm.Crc64Nvme;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumSha1, out checksum))
            response.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumSha256, out checksum))
            response.ChecksumAlgorithm = ChecksumAlgorithm.Sha256;

        if (checksum != null)
            response.Checksum = Convert.FromBase64String(checksum);

        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.ReadToDescendant("CopyPartResult");

        foreach (string name in XmlHelper.ReadElements(reader))
        {
            switch (name)
            {
                case "ETag":
                    response.ETag = reader.ReadString();
                    break;
                case "LastModified":
                    response.LastModified = ValueHelper.ParseDate(reader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
                case "ChecksumCRC32":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Crc32;
                    response.Checksum = Convert.FromBase64String(reader.ReadString());
                    break;
                case "ChecksumCRC32C":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Crc32C;
                    response.Checksum = Convert.FromBase64String(reader.ReadString());
                    break;
                case "ChecksumCRC64NVME":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Crc64Nvme;
                    response.Checksum = Convert.FromBase64String(reader.ReadString());
                    break;
                case "ChecksumSHA1":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
                    response.Checksum = Convert.FromBase64String(reader.ReadString());
                    break;
                case "ChecksumSHA256":
                    response.ChecksumAlgorithm = ChecksumAlgorithm.Sha256;
                    response.Checksum = Convert.FromBase64String(reader.ReadString());
                    break;
            }
        }
    }
}