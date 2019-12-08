using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Bucket
{
    [UsedImplicitly]
    internal class GetBucketLockConfigurationResponseMarshal : IResponseMarshal<GetBucketLockConfigurationRequest, GetBucketLockConfigurationResponse>
    {
        public void MarshalResponse(IS3Config config, GetBucketLockConfigurationRequest request, GetBucketLockConfigurationResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("DefaultRetention");

                while (xmlReader.Read())
                {
                    if (!xmlReader.IsStartElement())
                        continue;

                    switch (xmlReader.Name)
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