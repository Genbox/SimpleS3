using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class PutObjectLegalHoldRequestMarshal : IRequestMarshal<PutObjectLegalHoldRequest>
    {
        public Stream MarshalRequest(PutObjectLegalHoldRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.LegalHold, string.Empty);

            FastXmlWriter xml = new FastXmlWriter(512);
            xml.WriteStartElement("LegalHold", "http://s3.amazonaws.com/doc/2006-03-01/");
            xml.WriteElement("Status", request.LegalHold ? "ON" : "OFF");
            xml.WriteEndElement("LegalHold");

            return new MemoryStream(xml.GetBytes());
        }
    }
}