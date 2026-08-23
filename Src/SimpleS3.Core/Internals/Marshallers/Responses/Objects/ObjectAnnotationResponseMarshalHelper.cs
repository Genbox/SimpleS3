using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal static class ObjectAnnotationResponseMarshalHelper
{
    public static (string? VersionId, string? ETag, SseAlgorithm SseAlgorithm, string? SseKmsKeyId, ChecksumAlgorithm ChecksumAlgorithm, ChecksumType ChecksumType, byte[]? Checksum) MapHeaders(IDictionary<string, string> headers)
    {
        SseAlgorithm sseAlgorithm = SseAlgorithm.Unknown;
        if (headers.TryGetHeader(AmzHeaders.XAmzSse, out string? sseHeader))
            sseAlgorithm = Core.Enums.Enums.SseAlgorithm.Parse(sseHeader, SseAlgorithmFormat.DisplayName);

        ChecksumType checksumType = ChecksumType.Unknown;
        if (headers.TryGetHeader(AmzHeaders.XAmzChecksumType, out string? checksumTypeHeader))
            checksumType = Core.Enums.Enums.ChecksumType.Parse(checksumTypeHeader, ChecksumTypeFormat.DisplayName);

        ChecksumAlgorithm algorithm = ChecksumAlgorithm.Unknown;
        string? checksum = null;
        if (headers.TryGetHeader(AmzHeaders.XAmzChecksumCrc32, out checksum))
            algorithm = ChecksumAlgorithm.Crc32;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumCrc32C, out checksum))
            algorithm = ChecksumAlgorithm.Crc32C;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumCrc64Nvme, out checksum))
            algorithm = ChecksumAlgorithm.Crc64Nvme;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumSha1, out checksum))
            algorithm = ChecksumAlgorithm.Sha1;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumSha256, out checksum))
            algorithm = ChecksumAlgorithm.Sha256;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumSha512, out checksum))
            algorithm = ChecksumAlgorithm.Sha512;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumMd5, out checksum))
            algorithm = ChecksumAlgorithm.Md5;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumXxHash64, out checksum))
            algorithm = ChecksumAlgorithm.XxHash64;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumXxHash3, out checksum))
            algorithm = ChecksumAlgorithm.XxHash3;
        else if (headers.TryGetHeader(AmzHeaders.XAmzChecksumXxHash128, out checksum))
            algorithm = ChecksumAlgorithm.XxHash128;

        return (headers.GetOptionalValue(AmzHeaders.XAmzVersionId), headers.GetOptionalValue(HttpHeaders.ETag), sseAlgorithm,
                headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId), algorithm, checksumType,
                checksum == null ? null : Convert.FromBase64String(checksum));
    }
}