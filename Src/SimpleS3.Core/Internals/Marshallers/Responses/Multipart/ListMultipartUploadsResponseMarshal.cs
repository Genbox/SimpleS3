using System.Net;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;

internal sealed class ListMultipartUploadsResponseMarshal : IResponseMarshal<ListMultipartUploadsResponse>
{
    public void MarshalResponse(SimpleS3Config config, ListMultipartUploadsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);
        XmlHelper.ReadToDescendant(xmlReader, "ListMultipartUploadsResult");

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            switch (name)
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
                    response.EncodingType = Core.Enums.Enums.EncodingType.Parse(xmlReader.ReadString(), EncodingTypeFormat.DisplayName);
                    break;
                case "IsTruncated":
                    response.IsTruncated = ValueHelper.ParseBool(xmlReader.ReadString());
                    break;
                case "Prefix":
                    response.Prefix = xmlReader.ReadString();
                    break;
                case "Delimiter":
                    response.Delimiter = xmlReader.ReadString();
                    break;
                case "Upload":
                    ParseUpload(response, xmlReader);
                    break;
                case "CommonPrefixes":
                    ParseCommonPrefix(response, xmlReader);
                    break;
            }
        }

        if (response.EncodingType == EncodingType.Url)
        {
            if (response.KeyMarker != null)
                response.KeyMarker = Uri.UnescapeDataString(response.KeyMarker);
            if (response.NextKeyMarker != null)
                response.NextKeyMarker = Uri.UnescapeDataString(response.NextKeyMarker);
        }

        if (config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url)
        {
            response.Delimiter = WebUtility.UrlDecode(response.Delimiter);
            response.Prefix = WebUtility.UrlDecode(response.Prefix);
            foreach (S3Upload upload in response.Uploads)
                upload.ObjectKey = WebUtility.UrlDecode(upload.ObjectKey);
            for (int i = 0; i < response.CommonPrefixes.Count; i++)
                response.CommonPrefixes[i] = WebUtility.UrlDecode(response.CommonPrefixes[i]);
        }
    }

    private static void ParseUpload(ListMultipartUploadsResponse response, XmlReader xmlReader)
    {
        string? key = null;
        string? uploadId = null;
        S3Identity? initiator = null;
        S3Identity? owner = null;
        StorageClass storageClass = StorageClass.Unknown;
        DateTimeOffset? initiated = null;
        ChecksumAlgorithm checksumAlgorithm = ChecksumAlgorithm.Unknown;
        ChecksumType checksumType = ChecksumType.Unknown;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Upload"))
        {
            switch (name)
            {
                case "Key":
                    key = xmlReader.ReadString();
                    break;
                case "UploadId":
                    uploadId = xmlReader.ReadString();
                    break;
                case "Initiator":
                    initiator = ParserHelper.ParseOwner(xmlReader, "Initiator");
                    break;
                case "Owner":
                    owner = ParserHelper.ParseOwner(xmlReader);
                    break;
                case "StorageClass":
                    storageClass = Core.Enums.Enums.StorageClass.Parse(xmlReader.ReadString(), StorageClassFormat.DisplayName);
                    break;
                case "Initiated":
                    initiated = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
                case "ChecksumAlgorithm":
                    checksumAlgorithm = Core.Enums.Enums.ChecksumAlgorithm.Parse(xmlReader.ReadString(), ChecksumAlgorithmFormat.DisplayName);
                    break;
                case "ChecksumType":
                    checksumType = Core.Enums.Enums.ChecksumType.Parse(xmlReader.ReadString(), ChecksumTypeFormat.DisplayName);
                    break;
            }
        }

        if (key == null || uploadId == null || storageClass == StorageClass.Unknown || initiated == null)
            throw new InvalidOperationException("Missing required values");

        response.Uploads.Add(new S3Upload(key, uploadId, initiator, owner, storageClass, initiated.Value, checksumAlgorithm, checksumType));
    }

    private static void ParseCommonPrefix(ListMultipartUploadsResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "CommonPrefixes"))
        {
            if (name == "Prefix")
            {
                response.CommonPrefixes.Add(xmlReader.ReadString());
                return;
            }
        }
    }
}