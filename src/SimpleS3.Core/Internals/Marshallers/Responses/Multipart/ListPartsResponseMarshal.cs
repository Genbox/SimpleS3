using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class ListPartsResponseMarshal : IResponseMarshal<ListPartsResponse>
    {
        public void MarshalResponse(Config config, ListPartsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Iso8601DateTimeExt);
            response.AbortRuleId = headers.GetOptionalValue(AmzHeaders.XAmzAbortRuleId);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("ListPartsResult");

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    switch (xmlReader.Name)
                    {
                        case "Key":
                            response.ObjectKey = xmlReader.ReadString();
                            break;
                        case "Bucket":
                            response.BucketName = xmlReader.ReadString();
                            break;
                        case "UploadId":
                            response.UploadId = xmlReader.ReadString();
                            break;
                        case "EncodingType":
                            response.EncodingType = ValueHelper.ParseEnum<EncodingType>(xmlReader.ReadString());
                            break;
                        case "StorageClass":
                            response.StorageClass = ValueHelper.ParseEnum<StorageClass>(xmlReader.ReadString());
                            break;
                        case "PartNumberMarker":
                            response.PartNumberMarker = ValueHelper.ParseInt(xmlReader.ReadString());
                            break;
                        case "NextPartNumberMarker":
                            response.NextPartNumberMarker = ValueHelper.ParseInt(xmlReader.ReadString());
                            break;
                        case "MaxParts":
                            response.MaxParts = ValueHelper.ParseInt(xmlReader.ReadString());
                            break;
                        case "IsTruncated":
                            response.IsTruncated = ValueHelper.ParseBool(xmlReader.ReadString());
                            break;
                        case "Owner":
                            response.Owner = XmlHelper.ParseOwner(xmlReader);
                            break;
                        case "Initiator":
                            response.Initiator = XmlHelper.ParseOwner(xmlReader, "Initiator");
                            break;
                        case "Part":
                            ParsePart(response, xmlReader);
                            break;
                    }
                }
            }
        }

        private void ParsePart(ListPartsResponse response, XmlTextReader xmlReader)
        {
            int? partNumber = null;
            DateTimeOffset? lastModified = null;
            long? size = null;
            string? eTag = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Part")
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (xmlReader.Name)
                {
                    case "PartNumber":
                        partNumber = ValueHelper.ParseInt(xmlReader.ReadString());
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
                }
            }

            if (partNumber == null || lastModified == null || size == null)
                throw new InvalidOperationException("Missing required values");

            response.Parts.Add(new S3Part(partNumber.Value, lastModified.Value, size.Value, eTag));
        }
    }
}