using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Bucket
{
    [UsedImplicitly]
    internal class GetBucketTaggingResponseMarshal : IResponseMarshal<GetBucketTaggingRequest, GetBucketTaggingResponse>
    {
        public void MarshalResponse(IS3Config config, GetBucketTaggingRequest request, GetBucketTaggingResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.Tags = new Dictionary<string, string>();

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("TagSet");

                while (xmlReader.Read())
                {
                    if (!xmlReader.IsStartElement() || xmlReader.Name != "Tag")
                        continue;

                    //Read the next token
                    xmlReader.Read();

                    string key = xmlReader.ReadElementContentAsString();
                    string value = xmlReader.ReadElementContentAsString();
                    response.Tags.Add(key, value);
                }
            }
        }
    }
}