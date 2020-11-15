using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    [UsedImplicitly]
    internal class DeleteObjectsRequestMarshal : IRequestMarshal<DeleteObjectsRequest>
    {
        public Stream? MarshalRequest(DeleteObjectsRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.Delete, string.Empty);

            FastXmlWriter xml = new FastXmlWriter(512);
            xml.WriteStartElement("Delete");

            if (request.Quiet)
                xml.WriteElement("Quiet", true);

            foreach (S3DeleteInfo info in request.Objects)
            {
                xml.WriteStartElement("Object");
                xml.WriteElement("Key", info.Name);

                if (info.VersionId != null)
                    xml.WriteElement("VersionId", info.VersionId);

                xml.WriteEndElement("Object");
            }

            xml.WriteEndElement("Delete");
            return new MemoryStream(xml.GetBytes());
        }
    }
}