using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class ListObjectAnnotationsResponseMarshal : IResponseMarshal<ListObjectAnnotationsResponse>
{
    public void MarshalResponse(SimpleS3Config config, ListObjectAnnotationsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.ReadToDescendant("ListObjectAnnotationsOutput");
        foreach (string name in XmlHelper.ReadElements(reader))
        {
            switch (name)
            {
                case "AnnotationEntry":
                    response.Annotations.Add(ParseAnnotation(reader));
                    break;
                case "Bucket":
                    response.BucketName = reader.ReadString();
                    break;
                case "Key":
                    response.ObjectKey = reader.ReadString();
                    break;
                case "AnnotationPrefix":
                    response.AnnotationPrefix = reader.ReadString();
                    break;
                case "MaxAnnotationResults":
                    response.MaxAnnotationResults = ValueHelper.ParseInt(reader.ReadString());
                    break;
                case "AnnotationCount":
                    response.AnnotationCount = ValueHelper.ParseInt(reader.ReadString());
                    break;
                case "ContinuationToken":
                    response.ContinuationToken = reader.ReadString();
                    break;
                case "NextContinuationToken":
                    response.NextContinuationToken = reader.ReadString();
                    break;
            }
        }
    }

    private static S3Annotation ParseAnnotation(XmlReader reader)
    {
        string? annotationName = null;
        DateTimeOffset? lastModified = null;
        long? size = null;
        string? etag = null;
        ReplicationStatus replicationStatus = ReplicationStatus.Unknown;
        List<ChecksumAlgorithm> checksumAlgorithms = new List<ChecksumAlgorithm>();

        foreach (string name in XmlHelper.ReadElements(reader, "AnnotationEntry"))
        {
            switch (name)
            {
                case "AnnotationName": annotationName = reader.ReadString(); break;
                case "LastModified": lastModified = ValueHelper.ParseDate(reader.ReadString(), DateTimeFormat.Iso8601DateTimeExt); break;
                case "Size": size = ValueHelper.ParseLong(reader.ReadString()); break;
                case "ETag": etag = reader.ReadString(); break;
                case "ChecksumAlgorithm": checksumAlgorithms.Add(Core.Enums.Enums.ChecksumAlgorithm.Parse(reader.ReadString(), ChecksumAlgorithmFormat.DisplayName)); break;
                case "ReplicationStatus": replicationStatus = Core.Enums.Enums.ReplicationStatus.Parse(reader.ReadString(), ReplicationStatusFormat.DisplayName); break;
            }
        }

        if (annotationName == null || lastModified == null || size == null)
            throw new InvalidOperationException("Missing required values in AnnotationEntry");

        S3Annotation annotation = new S3Annotation(annotationName, lastModified.Value, size.Value)
        {
            ETag = etag,
            ReplicationStatus = replicationStatus
        };
        foreach (ChecksumAlgorithm algorithm in checksumAlgorithms)
            annotation.ChecksumAlgorithms.Add(algorithm);
        return annotation;
    }
}