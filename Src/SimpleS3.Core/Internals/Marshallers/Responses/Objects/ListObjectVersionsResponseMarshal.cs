using System.Net;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class ListObjectVersionsResponseMarshal : IResponseMarshal<ListObjectVersionsResponse>
{
    public void MarshalResponse(SimpleS3Config config, ListObjectVersionsResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
        {
            //To support Google's lack of versions which is identical to S3 ListObjectVersions, but not quite
            //xmlReader.ReadToDescendant("ListVersionsResult");

            foreach (string name in XmlHelper.ReadElements(xmlReader))
            {
                switch (name)
                {
                    case "IsTruncated":
                        response.IsTruncated = ValueHelper.ParseBool(xmlReader.ReadString());
                        break;
                    case "EncodingType":
                        response.EncodingType = Core.Enums.Enums.EncodingType.Parse(xmlReader.ReadString(), EncodingTypeFormat.DisplayName);
                        break;
                    case "KeyMarker":
                        response.KeyMarker = xmlReader.ReadString();
                        break;
                    case "VersionIdMarker":
                        response.VersionIdMarker = xmlReader.ReadString();
                        break;
                    case "NextKeyMarker":
                        response.NextKeyMarker = xmlReader.ReadString();
                        break;
                    case "NextVersionIdMarker":
                        response.NextVersionIdMarker = xmlReader.ReadString();
                        break;
                    case "Name":
                        response.BucketName = xmlReader.ReadString();
                        break;
                    case "Prefix":
                        response.Prefix = xmlReader.ReadString();
                        break;
                    case "Delimiter":
                        response.Delimiter = xmlReader.ReadString();
                        break;
                    case "MaxKeys":
                        response.MaxKeys = ValueHelper.ParseInt(xmlReader.ReadString());
                        break;
                    case "Version":
                        response.Versions.Add(ParseVersion(xmlReader));
                        break;
                    case "DeleteMarker":
                        response.DeleteMarkers.Add(ParseDeleteMarker(xmlReader));
                        break;
                    case "CommonPrefixes":
                        response.CommonPrefixes.Add(ParseCommonPrefixes(xmlReader));
                        break;
                }
            }
        }

        if (config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url)
        {
            response.KeyMarker = WebUtility.UrlDecode(response.KeyMarker);
            response.NextKeyMarker = WebUtility.UrlDecode(response.NextKeyMarker);
            response.Prefix = WebUtility.UrlDecode(response.Prefix);
            response.Delimiter = WebUtility.UrlDecode(response.Delimiter);

            foreach (S3Version version in response.Versions)
                version.ObjectKey = WebUtility.UrlDecode(version.ObjectKey);

            foreach (S3DeleteMarker marker in response.DeleteMarkers)
                marker.ObjectKey = WebUtility.UrlDecode(marker.ObjectKey);
        }
    }

    private static string ParseCommonPrefixes(XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "CommonPrefixes"))
        {
            if (name == "Prefix")
                return xmlReader.ReadString();
        }

        throw new InvalidOperationException("Could not parse common prefix");
    }

    private static S3Version ParseVersion(XmlReader xmlReader)
    {
        string? objectKey = null;
        string? versionId = null;
        bool? isLatest = null;
        DateTimeOffset? lastModified = null;
        string? etag = null;
        int size = -1;
        StorageClass storageClass = StorageClass.Unknown;
        S3Identity? owner = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Version"))
        {
            switch (name)
            {
                case "Key":
                    objectKey = xmlReader.ReadString();
                    break;
                case "VersionId":
                    versionId = xmlReader.ReadString();
                    break;
                case "IsLatest":
                    isLatest = ValueHelper.ParseBool(xmlReader.ReadString());
                    break;
                case "LastModified":
                    lastModified = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
                case "ETag":
                    etag = xmlReader.ReadString();
                    break;
                case "Size":
                    size = ValueHelper.ParseInt(xmlReader.ReadString());
                    break;
                case "StorageClass":
                    storageClass = Core.Enums.Enums.StorageClass.Parse(xmlReader.ReadString(), StorageClassFormat.DisplayName);
                    break;
                case "Owner":
                    owner = ParserHelper.ParseOwner(xmlReader);
                    break;
            }
        }

        if (objectKey == null || isLatest == null || lastModified == null || etag == null || size == -1)
            throw new InvalidOperationException("Missing required values");

        return new S3Version(objectKey, versionId, isLatest.Value, lastModified.Value, etag, size, owner, storageClass);
    }

    private static S3DeleteMarker ParseDeleteMarker(XmlReader xmlReader)
    {
        bool isLatest = false;
        string? versionId = null;
        string? objectKey = null;
        DateTimeOffset? lastModified = null;
        S3Identity? owner = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "DeleteMarker"))
        {
            switch (name)
            {
                case "Key":
                    objectKey = xmlReader.ReadString();
                    break;
                case "VersionId":
                    versionId = xmlReader.ReadString();
                    break;
                case "IsLatest":
                    isLatest = ValueHelper.ParseBool(xmlReader.ReadString());
                    break;
                case "LastModified":
                    lastModified = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
                case "Owner":
                    owner = ParserHelper.ParseOwner(xmlReader);
                    break;
            }
        }

        if (owner == null || objectKey == null || versionId == null || lastModified == null)
            throw new InvalidOperationException("Missing required values");

        return new S3DeleteMarker(isLatest, objectKey, lastModified.Value, owner, versionId);
    }
}