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
    internal class GetBucketAccelerateConfigurationResponseMarshal : IResponseMarshal<GetBucketAccelerateConfigurationResponse>
    {
        public void MarshalResponse(Config config, GetBucketAccelerateConfigurationResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("AccelerateConfiguration");

                if (xmlReader.Read())
                    response.AccelerateEnabled = xmlReader.ReadElementContentAsString() == "Enabled";
            }
        }
    }
}