using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;

internal sealed class CompleteMultipartUploadRequestMarshal : IRequestMarshal<CompleteMultipartUploadRequest>
{
    public Stream MarshalRequest(CompleteMultipartUploadRequest request, SimpleS3Config config)
    {
        //build the XML required to describe each part
        FastXmlWriter xml = new FastXmlWriter(512);
        xml.WriteStartElement("CompleteMultipartUpload");

        foreach (S3PartInfo partInfo in request.UploadParts)
        {
            xml.WriteStartElement("Part");

            if (partInfo.Checksum != null)
                xml.WriteElement("Checksum" + partInfo.ChecksumAlgorithm.GetDisplayName(), Convert.ToBase64String(partInfo.Checksum));

            xml.WriteElement("ETag", partInfo.ETag.Trim('"'));
            xml.WriteElement("PartNumber", partInfo.PartNumber.ToString(NumberFormatInfo.InvariantInfo));
            xml.WriteEndElement("Part");
        }

        xml.WriteEndElement("CompleteMultipartUpload");

        return new MemoryStream(xml.GetBytes());
    }
}