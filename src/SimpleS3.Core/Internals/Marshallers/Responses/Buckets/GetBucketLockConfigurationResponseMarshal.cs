using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets
{
    internal class GetBucketLockConfigurationResponseMarshal : IResponseMarshal<GetBucketLockConfigurationResponse>
    {
        public void MarshalResponse(SimpleS3Config config, GetBucketLockConfigurationResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("DefaultRetention");

                foreach (string name in XmlHelper.ReadElements(xmlReader))
                {
                    switch (name)
                    {
                        case "Mode":
                            response.LockMode = ValueHelper.ParseEnum<LockMode>(xmlReader.ReadString());
                            break;
                        case "Days":
                            response.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(xmlReader.ReadElementContentAsInt());
                            break;
                        case "Years":
                            response.LockRetainUntil = DateTimeOffset.UtcNow.AddYears(xmlReader.ReadElementContentAsInt());
                            break;
                    }
                }
            }
        }
    }
}