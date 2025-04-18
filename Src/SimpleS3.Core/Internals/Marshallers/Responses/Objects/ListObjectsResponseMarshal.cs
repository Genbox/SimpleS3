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
    public void MarshalResponse(SimpleS3Config config, ListObjectsResponse response, IDictionary<string, string> headers, Stream responseStream)
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
        ChecksumAlgorithm checksumAlgorithm = ChecksumAlgorithm.Unknown;
        ChecksumType checksumType = ChecksumType.Unknown;

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
                    storageClass = Core.Enums.Enums.StorageClass.Parse(xmlReader.ReadString(), StorageClassFormat.DisplayName);
                    break;
                case "Owner":
                    owner = ParserHelper.ParseOwner(xmlReader);
                    break;
                case "ChecksumAlgorithm":
                    checksumAlgorithm = Core.Enums.Enums.ChecksumAlgorithm.Parse(xmlReader.ReadString(), ChecksumAlgorithmFormat.DisplayName);
                    break;
                case "ChecksumType":
                    checksumType = Core.Enums.Enums.ChecksumType.Parse(xmlReader.ReadString(), ChecksumTypeFormat.DisplayName);
                    break;
            }
        }

        if (key == null || lastModified == null || size == null)
            throw new InvalidOperationException("Missing required values");

        response.Objects.Add(new S3Object(key, lastModified.Value, size.Value, owner, eTag, storageClass, checksumAlgorithm, checksumType));
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