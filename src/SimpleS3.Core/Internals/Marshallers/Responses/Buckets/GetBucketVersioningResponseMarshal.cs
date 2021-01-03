using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    [UsedImplicitly]
    internal class GetBucketVersioningResponseMarshal : IResponseMarshal<GetBucketVersioningResponse>
    {
        public void MarshalResponse(Config config, GetBucketVersioningResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    switch (xmlReader.Name)
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
}