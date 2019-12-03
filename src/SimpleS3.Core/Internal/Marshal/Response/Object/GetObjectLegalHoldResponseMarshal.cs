using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.Xml;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Object
{
    [UsedImplicitly]
    internal class GetObjectLegalHoldResponseMarshal : IResponseMarshal<GetObjectLegalHoldRequest, GetObjectLegalHoldResponse>
    {
        public void MarshalResponse(IS3Config config, GetObjectLegalHoldRequest request, GetObjectLegalHoldResponse response, IDictionary<string, string> headers, Stream responseStream)
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