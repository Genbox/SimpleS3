using System.Net;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart;

internal sealed class ListPartsResponseMarshal : IResponseMarshal<ListPartsResponse>
{
    public void MarshalResponse(SimpleS3Config config, ListPartsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.AbortsOn = headers.GetHeaderNullableDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Rfc1123);
        response.AbortRuleId = headers.GetOptionalValue(AmzHeaders.XAmzAbortRuleId);
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);
        XmlHelper.ReadToDescendant(xmlReader, "ListPartsResult");

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            switch (name)
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
                    response.EncodingType = Core.Enums.Enums.EncodingType.Parse(xmlReader.ReadString(), EncodingTypeFormat.DisplayName);
                    break;
                case "StorageClass":
                    response.StorageClass = Core.Enums.Enums.StorageClass.Parse(xmlReader.ReadString(), StorageClassFormat.DisplayName);
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
                    response.Owner = ParserHelper.ParseOwner(xmlReader);
                    break;
                case "Initiator":
                    response.Initiator = ParserHelper.ParseOwner(xmlReader, "Initiator");
                    break;
                case "ChecksumAlgorithm":
                    response.ChecksumAlgorithm = Core.Enums.Enums.ChecksumAlgorithm.Parse(xmlReader.ReadString(), ChecksumAlgorithmFormat.DisplayName);
                    break;
                case "ChecksumType":
                    response.ChecksumType = Core.Enums.Enums.ChecksumType.Parse(xmlReader.ReadString(), ChecksumTypeFormat.DisplayName);
                    break;
                case "Part":
                    ParsePart(response, xmlReader);
                    break;
            }
        }

        if (config.AutoUrlDecodeResponses && response.EncodingType == EncodingType.Url)
            response.ObjectKey = WebUtility.UrlDecode(response.ObjectKey);
    }

    private static void ParsePart(ListPartsResponse response, XmlReader xmlReader)
    {
        int? partNumber = null;
        DateTimeOffset? lastModified = null;
        long? size = null;
        string? eTag = null;
        byte[]? checksumCrc32 = null;
        byte[]? checksumCrc32C = null;
        byte[]? checksumCrc64Nvme = null;
        byte[]? checksumSha1 = null;
        byte[]? checksumSha256 = null;
        byte[]? checksumSha512 = null;
        byte[]? checksumMd5 = null;
        byte[]? checksumXxHash64 = null;
        byte[]? checksumXxHash3 = null;
        byte[]? checksumXxHash128 = null;

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Part"))
        {
            switch (name)
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
                case "ChecksumCRC32":
                    checksumCrc32 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumCRC32C":
                    checksumCrc32C = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumCRC64NVME":
                    checksumCrc64Nvme = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumSHA1":
                    checksumSha1 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumSHA256":
                    checksumSha256 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumSHA512":
                    checksumSha512 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumMD5":
                    checksumMd5 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumXXHASH64":
                    checksumXxHash64 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumXXHASH3":
                    checksumXxHash3 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumXXHASH128":
                    checksumXxHash128 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
            }
        }

        if (partNumber == null || lastModified == null || size == null)
            throw new InvalidOperationException("Missing required values");

        response.Parts.Add(new S3Part(partNumber.Value, lastModified.Value, size.Value, eTag)
        {
            ChecksumCrc32 = checksumCrc32,
            ChecksumCrc32C = checksumCrc32C,
            ChecksumCrc64Nvme = checksumCrc64Nvme,
            ChecksumSha1 = checksumSha1,
            ChecksumSha256 = checksumSha256,
            ChecksumSha512 = checksumSha512,
            ChecksumMd5 = checksumMd5,
            ChecksumXxHash64 = checksumXxHash64,
            ChecksumXxHash3 = checksumXxHash3,
            ChecksumXxHash128 = checksumXxHash128
        });
    }
}