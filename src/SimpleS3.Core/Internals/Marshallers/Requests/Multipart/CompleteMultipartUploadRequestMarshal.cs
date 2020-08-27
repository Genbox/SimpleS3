using System.Globalization;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart
{
    [UsedImplicitly]
    internal class CompleteMultipartUploadRequestMarshal : IRequestMarshal<CompleteMultipartUploadRequest>
    {
        public Stream? MarshalRequest(CompleteMultipartUploadRequest request, IConfig config)
        {
            //build the XML required to describe each part
            FastXmlWriter xml = new FastXmlWriter(512);
            xml.WriteStartElement("CompleteMultipartUpload");

            foreach (S3PartInfo partInfo in request.UploadParts)
            {
                xml.WriteStartElement("Part");
                xml.WriteElement("ETag", partInfo.ETag.Trim('"'));
                xml.WriteElement("PartNumber", partInfo.PartNumber.ToString(NumberFormatInfo.InvariantInfo));
                xml.WriteEndElement("Part");
            }

            xml.WriteEndElement("CompleteMultipartUpload");

            return new MemoryStream(xml.GetBytes());
        }
    }
}