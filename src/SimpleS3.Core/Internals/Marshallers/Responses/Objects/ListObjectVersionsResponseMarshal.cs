using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class ListObjectVersionsResponseMarshal : IResponseMarshal<ListObjectVersionsResponse>
    {
        public void MarshalResponse(Config config, ListObjectVersionsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.Versions = new List<S3Version>();
            response.DeleteMarkers = new List<S3DeleteMarker>();
            response.CommonPrefixes = new List<string>();

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                while (r.Read())
                {
                    if (r.NodeType != XmlNodeType.Element)
                        continue;

                    switch (r.Name)
                    {
                        case "IsTruncated":
                            response.IsTruncated = ValueHelper.ParseBool(r.ReadString());
                            break;
                        case "EncodingType":
                            response.EncodingType = ValueHelper.ParseEnum<EncodingType>(r.ReadString());
                            break;
                        case "KeyMarker":
                            response.KeyMarker = r.ReadString();
                            break;
                        case "VersionIdMarker":
                            response.VersionIdMarker = r.ReadString();
                            break;
                        case "NextKeyMarker":
                            response.NextKeyMarker = r.ReadString();
                            break;
                        case "NextVersionIdMarker":
                            response.NextVersionIdMarker = r.ReadString();
                            break;
                        case "Name":
                            response.Name = r.ReadString();
                            break;
                        case "Prefix":
                            response.Prefix = r.ReadString();
                            break;
                        case "Delimiter":
                            response.Delimiter = r.ReadString();
                            break;
                        case "MaxKeys":
                            response.MaxKeys = ValueHelper.ParseInt(r.ReadString());
                            break;
                        case "Version":
                            response.Versions.Add(ParseVersion(r));
                            break;
                        case "DeleteMarker":
                            response.DeleteMarkers.Add(ParseDeleteMarker(r));
                            break;
                        case "CommonPrefixes":
                            response.CommonPrefixes.Add(ParseCommonPrefixes(r));
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
                {
                    version.ObjectKey = WebUtility.UrlDecode(version.ObjectKey);
                }

                foreach (S3DeleteMarker marker in response.DeleteMarkers)
                {
                    marker.ObjectKey = WebUtility.UrlDecode(marker.ObjectKey);
                }
            }
        }

        private static string ParseCommonPrefixes(XmlTextReader r)
        {
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement && r.Name == "CommonPrefixes")
                    break;

                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case "Prefix":
                        return r.ReadString();
                }
            }

            throw new InvalidOperationException("Could not parse common prefix");
        }

        private static S3Version ParseVersion(XmlTextReader r)
        {
            S3Version version = new S3Version();

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement && r.Name == "Version")
                    break;

                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case "Key":
                        version.ObjectKey = r.ReadString();
                        break;
                    case "VersionId":
                        version.VersionId = r.ReadString();
                        break;
                    case "IsLatest":
                        version.IsLatest = ValueHelper.ParseBool(r.ReadString());
                        break;
                    case "LastModified":
                        version.LastModified = ValueHelper.ParseDate(r.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                    case "ETag":
                        version.Etag = r.ReadString();
                        break;
                    case "Size":
                        version.Size = ValueHelper.ParseInt(r.ReadString());
                        break;
                    case "StorageClass":
                        version.StorageClass = ValueHelper.ParseEnum<StorageClass>(r.ReadString());
                        break;
                    case "Owner":
                        version.Owner = ParseOwner(r);
                        break;
                }
            }

            return version;
        }

        private static Owner ParseOwner(XmlTextReader r)
        {
            Owner owner = new Owner();

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement && r.Name == "Owner")
                    break;

                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case "DisplayName":
                        owner.DisplayName = r.ReadString();
                        break;
                    case "ID":
                        owner.Id = r.ReadString();
                        break;
                }
            }

            return owner;
        }

        private static S3DeleteMarker ParseDeleteMarker(XmlTextReader r)
        {
            bool isLatest = false;
            string? versionId = null;
            string? objectKey = null;
            DateTimeOffset lastModified;
            Owner? owner = null;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement && r.Name == "DeleteMarker")
                    break;

                if (r.NodeType != XmlNodeType.Element)
                    continue;

                switch (r.Name)
                {
                    case "Key":
                        objectKey = r.ReadString();
                        break;
                    case "VersionId":
                        versionId = r.ReadString();
                        break;
                    case "IsLatest":
                        isLatest = ValueHelper.ParseBool(r.ReadString());
                        break;
                    case "LastModified":
                        lastModified = ValueHelper.ParseDate(r.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                    case "Owner":
                        owner = ParseOwner(r);
                        break;
                }
            }

            if (owner == null || objectKey == null || versionId == null)
                throw new ArgumentNullException();

            return new S3DeleteMarker(isLatest, objectKey, lastModified, owner, versionId);
        }
    }
}