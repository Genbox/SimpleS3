using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class CreateBucketRequestMarshal : IRequestMarshal<CreateBucketRequest>
    {
        public Stream MarshalRequest(CreateBucketRequest request, IConfig config)
        {
            //We only enable object locking on creation. We can't disable it, so there is no "false" option
            if (request.EnableObjectLocking.HasValue && request.EnableObjectLocking.Value)
                request.AddHeader(AmzHeaders.XAmzBucketObjectLockEnabled, request.EnableObjectLocking.Value ? "TRUE" : string.Empty);

            //Hard-code the LocationConstraint to the region from the config
            FastXmlWriter writer = new FastXmlWriter(128);
            writer.WriteStartElement("CreateBucketConfiguration");
            writer.WriteElement("LocationConstraint", ValueHelper.EnumToString(config.Region));
            writer.WriteEndElement("CreateBucketConfiguration");

            return new MemoryStream(writer.GetBytes());
        }
    }
}