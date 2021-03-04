using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class GetObjectLegalHoldResponseMarshal : IResponseMarshal<GetObjectLegalHoldResponse>
    {
        public void MarshalResponse(Config config, GetObjectLegalHoldResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            using (XmlTextReader reader = new XmlTextReader(responseStream))
            {
                reader.ReadToDescendant("Status");
                reader.Read();
                response.LegalHold = reader.Value == "ON";
            }
        }
    }
}