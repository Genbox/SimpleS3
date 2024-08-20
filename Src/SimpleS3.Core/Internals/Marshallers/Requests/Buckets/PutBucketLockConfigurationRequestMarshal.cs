using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketLockConfigurationRequestMarshal : IRequestMarshal<PutBucketLockConfigurationRequest>
{
    public Stream MarshalRequest(PutBucketLockConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.ObjectLock, string.Empty);

        FastXmlWriter writer = new FastXmlWriter(128);
        writer.WriteStartElement("ObjectLockConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("ObjectLockEnabled", "Enabled");

        if (request.LockMode != LockMode.Unknown)
        {
            writer.WriteStartElement("Rule");
            writer.WriteStartElement("DefaultRetention");
            writer.WriteElement("Mode", ValueHelper.EnumToString(request.LockMode));

            if (request.LockRetainUntil.HasValue)
                writer.WriteElement("Days", (request.LockRetainUntil.Value - DateTimeOffset.UtcNow).Days);

            writer.WriteEndElement("DefaultRetention");
            writer.WriteEndElement("Rule");
        }

        writer.WriteEndElement("ObjectLockConfiguration");

        return new MemoryStream(writer.GetBytes());
    }
}