using System.Net;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class ListObjectsResponseMarshal : IResponseMarshal<ListObjectsResponse>
{
    public void MarshalResponse(SimpleS3Config config, ListObjectsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using (XmlReader xmlReader = XmlHelper.CreateReader(responseStream))
        {
            xmlReader.MoveToContent();

            foreach (string name in XmlHelper.ReadChildren(xmlReader))
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
                        response.IsTruncated = XmlConvert.ToBoolean(xmlReader.ReadString());
                        break;
                    case "EncodingType":
                        response.EncodingType = Core.Enums.Enums.EncodingType.Parse(xmlReader.ReadString(), EncodingTypeFormat.DisplayName);
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
                obj.ObjectKey = WebUtility.UrlDecode(obj.ObjectKey);

            for (int i = 0; i < response.CommonPrefixes.Count; i++)
                response.CommonPrefixes[i] = WebUtility.UrlDecode(response.CommonPrefixes[i]);
        }
    }

    private static void ReadContents(ListObjectsResponse response, XmlReader xmlReader)
    {
        string? key = null;
        DateTimeOffset lastModified = default;
        string? eTag = null;
        long size = default;
        StorageClass storageClass = StorageClass.Unknown;
        S3Identity? owner = null;
        List<ChecksumAlgorithm> checksumAlgorithms = [];
        ChecksumType checksumType = ChecksumType.Unknown;
        S3RestoreStatus? restoreStatus = null;

        foreach (string name in XmlHelper.ReadChildren(xmlReader))
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
                    storageClass = Core.Enums.Enums.StorageClass.Parse(xmlReader.ReadString(), StorageClassFormat.DisplayName);
                    break;
                case "Owner":
                    owner = ParserHelper.ParseOwner(xmlReader, requireId: false);
                    break;
                case "ChecksumAlgorithm":
                    checksumAlgorithms.Add(Core.Enums.Enums.ChecksumAlgorithm.Parse(xmlReader.ReadString(), ChecksumAlgorithmFormat.DisplayName));
                    break;
                case "ChecksumType":
                    checksumType = Core.Enums.Enums.ChecksumType.Parse(xmlReader.ReadString(), ChecksumTypeFormat.DisplayName);
                    break;
                case "RestoreStatus":
                    restoreStatus = ParserHelper.ParseRestoreStatus(xmlReader);
                    break;
            }
        }

        response.Objects.Add(new S3Object(key ?? string.Empty, lastModified, size, owner, eTag, storageClass, checksumAlgorithms, checksumType) { RestoreStatus = restoreStatus });
    }

    private static void ReadCommonPrefixes(ListObjectsResponse response, XmlReader xmlReader)
    {
        string? key = null;

        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            if (name == "Prefix")
                key = xmlReader.ReadString();
        }

        response.CommonPrefixes.Add(key ?? string.Empty);
    }
}