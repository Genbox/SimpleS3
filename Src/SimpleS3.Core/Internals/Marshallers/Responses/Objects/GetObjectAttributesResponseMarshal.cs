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

internal sealed class GetObjectAttributesResponseMarshal : IResponseMarshal<GetObjectAttributesResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetObjectAttributesResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        response.IsDeleteMarker = headers.GetHeaderBool(AmzHeaders.XAmzDeleteMarker);
        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);
        response.LastModified = headers.GetHeaderNullableDate(HttpHeaders.LastModified, DateTimeFormat.Rfc1123);

        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);
        xmlReader.MoveToContent();
        if (xmlReader.LocalName != "GetObjectAttributesResponse")
            throw new InvalidOperationException($"Expected XML root 'GetObjectAttributesResponse', but found '{xmlReader.LocalName}'.");

        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            switch (name)
            {
                case "ETag":
                    response.ETag = xmlReader.ReadString();
                    break;
                case "Checksum":
                    ParseChecksum(response, xmlReader);
                    break;
                case "ObjectParts":
                    ParseObjectParts(response, xmlReader);
                    break;
                case "StorageClass":
                    response.StorageClass = Core.Enums.Enums.StorageClass.Parse(xmlReader.ReadString(), StorageClassFormat.DisplayName);
                    break;
                case "ObjectSize":
                    response.ObjectSize = ValueHelper.ParseLong(xmlReader.ReadString());
                    break;
            }
        }
    }

    private static void ParseChecksum(GetObjectAttributesResponse response, XmlReader xmlReader)
    {
        bool composite = false;

        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            switch (name)
            {
                case "ChecksumCRC32":
                    response.ChecksumCrc32 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumCRC32C":
                    response.ChecksumCrc32C = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumCRC64NVME":
                    response.ChecksumCrc64Nvme = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumSHA1":
                    response.ChecksumSha1 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumSHA256":
                    response.ChecksumSha256 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumSHA512":
                    response.ChecksumSha512 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumMD5":
                    response.ChecksumMd5 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumXXHASH64":
                    response.ChecksumXxHash64 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumXXHASH3":
                    response.ChecksumXxHash3 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumXXHASH128":
                    response.ChecksumXxHash128 = ParseObjectChecksum(xmlReader.ReadString(), ref composite);
                    break;
                case "ChecksumType":
                    response.ChecksumType = Core.Enums.Enums.ChecksumType.Parse(xmlReader.ReadString(), ChecksumTypeFormat.DisplayName);
                    break;
            }
        }

        if (response.ChecksumType == ChecksumType.Unknown && composite)
            response.ChecksumType = ChecksumType.Composite;
    }

    private static byte[] ParseObjectChecksum(string value, ref bool composite)
    {
        byte[] checksum = ChecksumHelper.Decode(value, out bool valueIsComposite);
        composite |= valueIsComposite;
        return checksum;
    }

    private static void ParseObjectParts(GetObjectAttributesResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            switch (name)
            {
                case "PartsCount":
                    response.TotalPartsCount = ValueHelper.ParseInt(xmlReader.ReadString());
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
                case "Part":
                    response.Parts.Add(ParsePart(xmlReader));
                    break;
            }
        }
    }

    private static S3ObjectAttributePart ParsePart(XmlReader xmlReader)
    {
        int? partNumber = null;
        long? size = null;
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

        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            switch (name)
            {
                case "PartNumber":
                    partNumber = ValueHelper.ParseInt(xmlReader.ReadString());
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

        if (partNumber == null || size == null)
            throw new InvalidOperationException("Missing required values");

        return new S3ObjectAttributePart(partNumber.Value, size.Value)
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
        };
    }
}