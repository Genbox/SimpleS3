using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class ListMultipartUploadsResponseMarshal : IResponseMarshal<ListMultipartUploadsResponse>
    {
        public void MarshalResponse(Config config, ListMultipartUploadsResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("ListMultipartUploadsResult");

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    switch (xmlReader.Name)
                    {
                        case "Bucket":
                            response.Bucket = xmlReader.ReadString();
                            break;
                        case "KeyMarker":
                            response.KeyMarker = xmlReader.ReadString();
                            break;
                        case "UploadIdMarker":
                            response.UploadIdMarker = xmlReader.ReadString();
                            break;
                        case "NextKeyMarker":
                            response.NextKeyMarker = xmlReader.ReadString();
                            break;
                        case "NextUploadIdMarker":
                            response.NextUploadIdMarker = xmlReader.ReadString();
                            break;
                        case "MaxUploads":
                            response.MaxUploads = ValueHelper.ParseInt(xmlReader.ReadString());
                            break;
                        case "EncodingType":
                            response.EncodingType = ValueHelper.ParseEnum<EncodingType>(xmlReader.ReadString());
                            break;
                        case "IsTruncated":
                            response.IsTruncated = ValueHelper.ParseBool(xmlReader.ReadString());
                            break;
                        case "Upload":
                            ParseUpload(response, xmlReader);
                            break;
                    }
                }
            }
        }

        private void ParseUpload(ListMultipartUploadsResponse response, XmlTextReader xmlReader)
        {
            string? key = null;
            string? uploadId = null;
            S3Identity? initiator = null;
            S3Identity? owner = null;
            StorageClass storageClass = StorageClass.Unknown;
            DateTimeOffset? initiated = null;

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Upload")
                    break;

                if (xmlReader.NodeType != XmlNodeType.Element)
                    continue;

                switch (xmlReader.Name)
                {
                    case "Key":
                        key = xmlReader.ReadString();
                        break;
                    case "UploadId":
                        uploadId = xmlReader.ReadString();
                        break;
                    case "Initiator":
                        initiator = XmlHelper.ParseOwner(xmlReader, "Initiator");
                        break;
                    case "Owner":
                        owner = XmlHelper.ParseOwner(xmlReader);
                        break;
                    case "StorageClass":
                        storageClass = ValueHelper.ParseEnum<StorageClass>(xmlReader.ReadString());
                        break;
                    case "Initiated":
                        initiated = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                }
            }

            if (key == null || uploadId == null || initiator == null || owner == null || storageClass == StorageClass.Unknown || initiated == null)
                throw new InvalidOperationException("Missing required values");

            response.Uploads.Add(new S3Upload(key, uploadId, initiator, owner, storageClass, initiated.Value));
        }
    }
}