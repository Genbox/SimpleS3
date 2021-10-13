using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class ListObjectsResponseMarshal : IResponseMarshal<ListObjectsResponse>
    {
        public void MarshalResponse(Config config, ListObjectsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("ListBucketResult");

                foreach (string name in XmlHelper.ReadElements(xmlReader))
                {
                    switch (name)
                    {
                        case "Name":
                            response.BucketName = xmlReader.ReadString();
                            break;
                        case "Prefix":
                            response.Prefix = xmlReader.ReadString();
                            break;
                        case "KeyCount":
                            response.KeyCount = ValueHelper.ParseInt(xmlReader.ReadString());
                            break;
                        case "MaxKeys":
                            response.MaxKeys = ValueHelper.ParseInt(xmlReader.ReadString());
                            break;
                        case "IsTruncated":
                            response.IsTruncated = ValueHelper.ParseBool(xmlReader.ReadString());
                            break;
                        case "EncodingType":
                            response.EncodingType = ValueHelper.ParseEnum<EncodingType>(xmlReader.ReadString());
                            break;
                        case "Delimiter":
                            response.Delimiter = xmlReader.ReadString();
                            break;
                        case "StartAfter":
                            response.StartAfter = xmlReader.ReadString();
                            break;
                        case "NextContinuationToken":
                            response.NextContinuationToken = xmlReader.ReadString();
                            break;
                        case "ContinuationToken":
                            response.ContinuationToken = xmlReader.ReadString();
                            break;
                        case "Contents":
                            ReadContents(response, xmlReader);
                            break;
                        case "CommonPrefixes":
                            ReadCommonPrefixes(response, xmlReader);
                            break;
                    }
                }
            }

            if (config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url)
            {
                response.Delimiter = WebUtility.UrlDecode(response.Delimiter);
                response.Prefix = WebUtility.UrlDecode(response.Prefix);
                response.StartAfter = WebUtility.UrlDecode(response.StartAfter);

                foreach (S3Object obj in response.Objects)
                {
                    obj.ObjectKey = WebUtility.UrlDecode(obj.ObjectKey);
                }
            }
        }

        private static void ReadContents(ListObjectsResponse response, XmlReader xmlReader)
        {
            string? key = null;
            DateTimeOffset? lastModified = null;
            string? eTag = null;
            long? size = null;
            StorageClass storageClass = StorageClass.Unknown;
            S3Identity? owner = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "Contents"))
            {
                switch (name)
                {
                    case "Key":
                        key = xmlReader.ReadString();
                        break;
                    case "LastModified":
                        lastModified = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                    case "ETag":
                        eTag = xmlReader.ReadString();
                        break;
                    case "Size":
                        size = ValueHelper.ParseLong(xmlReader.ReadString());
                        break;
                    case "StorageClass":
                        storageClass = ValueHelper.ParseEnum<StorageClass>(xmlReader.ReadString());
                        break;
                    case "Owner":
                        owner = ParserHelper.ParseOwner(xmlReader);
                        break;
                }
            }

            if (key == null || lastModified == null || size == null)
                throw new InvalidOperationException("Missing required values");

            response.Objects.Add(new S3Object(key, lastModified.Value, size.Value, owner, eTag, storageClass));
        }

        private static void ReadCommonPrefixes(ListObjectsResponse response, XmlReader xmlReader)
        {
            string? key = null;

            foreach (string name in XmlHelper.ReadElements(xmlReader, "CommonPrefixes"))
            {
                if (name == "Prefix")
                    key = xmlReader.ReadString();
            }

            if (key == null)
                throw new InvalidOperationException("Missing required values");

            response.CommonPrefixes.Add(key);
        }
    }
}