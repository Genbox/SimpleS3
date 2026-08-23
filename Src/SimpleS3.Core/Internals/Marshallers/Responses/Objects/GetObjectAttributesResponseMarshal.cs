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

        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);
        xmlReader.ReadToDescendant("GetObjectAttributesResponse");

        foreach (string name in XmlHelper.ReadElements(xmlReader, "GetObjectAttributesResponse"))
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
                case "LastModified":
                    response.LastModified = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
            }
        }
    }

    private static void ParseChecksum(GetObjectAttributesResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "Checksum"))
        {
            switch (name)
            {
                case "ChecksumCRC32":
                    response.ChecksumCrc32 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumCRC32C":
                    response.ChecksumCrc32C = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumCRC64NVME":
                    response.ChecksumCrc64Nvme = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumSHA1":
                    response.ChecksumSha1 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumSHA256":
                    response.ChecksumSha256 = Convert.FromBase64String(xmlReader.ReadString());
                    break;
                case "ChecksumType":
                    response.ChecksumType = Core.Enums.Enums.ChecksumType.Parse(xmlReader.ReadString(), ChecksumTypeFormat.DisplayName);
                    break;
            }
        }
    }

    private static void ParseObjectParts(GetObjectAttributesResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadElements(xmlReader, "ObjectParts"))
        {
            switch (name)
            {
                case "TotalPartsCount":
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

        foreach (string name in XmlHelper.ReadElements(xmlReader, "Part"))
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
            ChecksumSha256 = checksumSha256
        };
    }
}