using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    internal class PutBucketVersioningResponseMarshal : IResponseMarshal<PutBucketVersioningResponse>
    {
        public void MarshalResponse(Config config, PutBucketVersioningResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            // The docs says there is an XML response, but there is none from the API.

            //using (XmlTextReader reader = new XmlTextReader(responseStream))
            //{
            //    reader.Namespaces = false;

            //    while (reader.Read())
            //    {
            //        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Status")
            //        {
            //            response.Status = ValueHelper.ParseBool(reader.ReadString());
            //            break;
            //        }
            //    }
            //}
        }
    }
}