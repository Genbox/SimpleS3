using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;

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

        (ChecksumAlgorithm algorithm, byte[]? checksum) = ChecksumHelper.ReadHeaders(headers);

        return (headers.GetOptionalValue(AmzHeaders.XAmzVersionId), headers.GetOptionalValue(HttpHeaders.ETag), sseAlgorithm,
                headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId), algorithm, checksumType,
                checksum);
    }
}