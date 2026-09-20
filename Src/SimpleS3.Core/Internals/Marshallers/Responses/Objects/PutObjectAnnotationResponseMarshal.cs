using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class PutObjectAnnotationResponseMarshal : IResponseMarshal<PutObjectAnnotationResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutObjectAnnotationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        (string? VersionId, string? ETag, Core.Enums.SseAlgorithm SseAlgorithm, string? SseKmsKeyId, Core.Enums.ChecksumAlgorithm ChecksumAlgorithm, Core.Enums.ChecksumType ChecksumType, byte[]? Checksum) data = ObjectAnnotationResponseMarshalHelper.MapHeaders(headers);
        response.VersionId = data.VersionId;
        response.ETag = data.ETag;
        response.SseAlgorithm = data.SseAlgorithm;
        response.SseKmsKeyId = data.SseKmsKeyId;
        response.ChecksumAlgorithm = data.ChecksumAlgorithm;
        response.ChecksumType = data.ChecksumType;
        response.Checksum = data.Checksum;
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.ReadToDescendant("PutObjectAnnotationOutput");

        foreach (string name in XmlHelper.ReadElements(reader))
        {
            if (name == "Key")
                response.ObjectKey = reader.ReadString();
            else if (name == "AnnotationName")
                response.AnnotationName = reader.ReadString();
        }
    }
}