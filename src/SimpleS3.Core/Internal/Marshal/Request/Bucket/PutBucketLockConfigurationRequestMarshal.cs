using System;
using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Internal.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Bucket
{
    [UsedImplicitly]
    internal class PutBucketLockConfigurationRequestMarshal : IRequestMarshal<PutBucketLockConfigurationRequest>
    {
        public Stream MarshalRequest(PutBucketLockConfigurationRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.ObjectLock, string.Empty);

            FastXmlWriter writer = new FastXmlWriter(128);
            writer.WriteStartElement("ObjectLockConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
            writer.WriteElement("ObjectLockEnabled", "Enabled");
            writer.WriteStartElement("Rule");
            writer.WriteStartElement("DefaultRetention");
            writer.WriteElement("Mode", ValueHelper.EnumToString(request.LockMode));
            writer.WriteElement("Days", (request.LockRetainUntil.Value - DateTimeOffset.UtcNow).Days);
            writer.WriteEndElement("DefaultRetention");
            writer.WriteEndElement("Rule");
            writer.WriteEndElement("ObjectLockConfiguration");

            return new MemoryStream(writer.GetBytes());
        }
    }
}