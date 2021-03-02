using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class CreateBucketRequestMarshal : IRequestMarshal<CreateBucketRequest>
    {
        public Stream? MarshalRequest(CreateBucketRequest request, Config config)
        {
            //We only enable object locking on creation. We can't disable it, so there is no "false" option
            if (request.EnableObjectLocking.HasValue && request.EnableObjectLocking.Value)
                request.SetHeader(AmzHeaders.XAmzBucketObjectLockEnabled, request.EnableObjectLocking.Value ? "TRUE" : string.Empty);

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
}