using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    [UsedImplicitly]
    internal class PutObjectLegalHoldRequestMarshal : IRequestMarshal<PutObjectLegalHoldRequest>
    {
        public Stream MarshalRequest(PutObjectLegalHoldRequest request, IConfig config)
        {
            request.AddQueryParameter(AmzParameters.LegalHold, string.Empty);

            FastXmlWriter xml = new FastXmlWriter(512);
            xml.WriteStartElement("LegalHold", "http://s3.amazonaws.com/doc/2006-03-01/");
            xml.WriteElement("Status", request.LockLegalHold.Value ? "ON" : "OFF");
            xml.WriteEndElement("LegalHold");

            return new MemoryStream(xml.GetBytes());
        }
    }
}