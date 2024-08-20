using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class CreateBucketRequestMarshal : IRequestMarshal<CreateBucketRequest>
{
    public Stream? MarshalRequest(CreateBucketRequest request, SimpleS3Config config)
    {
        //We only enable object locking on creation. We can't disable it, so there is no "false" option
        if (request.EnableObjectLocking == true)
            request.SetHeader(AmzHeaders.XAmzBucketObjectLockEnabled, "TRUE");

        //Hard-code the LocationConstraint to the region from the config
        if (config.ProviderName != "BackBlazeB2")
        {
            FastXmlWriter writer = new FastXmlWriter(128);
            writer.WriteStartElement("CreateBucketConfiguration");
            writer.WriteElement("LocationConstraint", config.RegionCode);
            writer.WriteEndElement("CreateBucketConfiguration");

            return new MemoryStream(writer.GetBytes());
        }

        return null;
    }
}