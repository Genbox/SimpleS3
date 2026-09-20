using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class GetObjectAnnotationResponseMarshal : IResponseMarshal<GetObjectAnnotationResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetObjectAnnotationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        (string? VersionId, string? ETag, Core.Enums.SseAlgorithm SseAlgorithm, string? SseKmsKeyId, Core.Enums.ChecksumAlgorithm ChecksumAlgorithm, Core.Enums.ChecksumType ChecksumType, byte[]? Checksum) data = ObjectAnnotationResponseMarshalHelper.MapHeaders(headers);
        response.VersionId = data.VersionId;
        response.ETag = data.ETag;
        response.SseAlgorithm = data.SseAlgorithm;
        response.SseKmsKeyId = data.SseKmsKeyId;
        response.ChecksumAlgorithm = data.ChecksumAlgorithm;
        response.ChecksumType = data.ChecksumType;
        response.Checksum = data.Checksum;
        response.LastModified = headers.GetHeaderDate(HttpHeaders.LastModified, DateTimeFormat.Rfc1123);
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        if (headers.TryGetHeader(AmzHeaders.XAmzReplicationStatus, out string? replicationStatus))
            response.ReplicationStatus = Core.Enums.Enums.ReplicationStatus.Parse(replicationStatus, ReplicationStatusFormat.DisplayName);
        response.Content = responseStream;
    }
}