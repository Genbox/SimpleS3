using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;

internal sealed class CompleteMultipartUploadRequestMarshal : IRequestMarshal<CompleteMultipartUploadRequest>
{
    public Stream MarshalRequest(CompleteMultipartUploadRequest request, SimpleS3Config config)
    {
        if ((string.Equals(config.ProviderName, "GoogleCloudStorage", StringComparison.OrdinalIgnoreCase) || string.Equals(config.ProviderName, "Wasabi", StringComparison.OrdinalIgnoreCase)) && (request.IfMatch != null || request.IfNoneMatch != null))
            throw new NotSupportedException($"{config.ProviderName} does not enforce CompleteMultipartUpload conditions. The completion was not sent.");

        request.SetOptionalHeader(HttpHeaders.IfMatch, request.IfMatch);
        request.SetOptionalHeader(HttpHeaders.IfNoneMatch, request.IfNoneMatch);
        if (request.ChecksumType != ChecksumType.Unknown)
            request.SetHeader(AmzHeaders.XAmzChecksumType, request.ChecksumType.GetDisplayName());
        if (request.MpuObjectSize.HasValue)
            request.SetHeader("x-amz-mp-object-size", request.MpuObjectSize.Value.ToString(CultureInfo.InvariantCulture));

        //build the XML required to describe each part
        FastXmlWriter xml = new FastXmlWriter(512);
        xml.WriteStartElement("CompleteMultipartUpload");

        foreach (S3PartInfo partInfo in request.UploadParts)
        {
            xml.WriteStartElement("Part");

            if (partInfo.Checksum != null)
                xml.WriteElement("Checksum" + partInfo.ChecksumAlgorithm.GetDisplayName(), Convert.ToBase64String(partInfo.Checksum));

            xml.WriteElement("ETag", partInfo.ETag);
            xml.WriteElement("PartNumber", partInfo.PartNumber.ToString(NumberFormatInfo.InvariantInfo));
            xml.WriteEndElement("Part");
        }

        xml.WriteEndElement("CompleteMultipartUpload");

        return new MemoryStream(xml.GetBytes());
    }
}