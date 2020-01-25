using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.Xml;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class GetObjectLegalHoldResponseMarshal : IResponseMarshal<GetObjectLegalHoldRequest, GetObjectLegalHoldResponse>
    {
        public void MarshalResponse(IConfig config, GetObjectLegalHoldRequest request, GetObjectLegalHoldResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            XmlSerializer s = new XmlSerializer(typeof(LegalHold));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                LegalHold legalHoldOutput = (LegalHold)s.Deserialize(r);
                response.LegalHold = legalHoldOutput.Status == "ON";
            }
        }
    }
}