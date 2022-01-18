using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal class GetBucketVersioningResponseMarshal : IResponseMarshal<GetBucketVersioningResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketVersioningResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
        {
            foreach (string name in XmlHelper.ReadElements(xmlReader))
            {
                switch (name)
                {
                    case "Status":
                        response.Status = xmlReader.ReadString() == "Enabled";
                        break;
                    case "MfaDelete":
                        response.MfaDelete = xmlReader.ReadString() == "Enabled";
                        break;
                }
            }
        }
    }
}