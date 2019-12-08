using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Internal.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Bucket
{
    [UsedImplicitly]
    internal class CreateBucketRequestMarshal : IRequestMarshal<CreateBucketRequest>
    {
        public Stream MarshalRequest(CreateBucketRequest request, IS3Config config)
        {
            request.AddHeader(AmzHeaders.XAmzBucketObjectLockEnabled, request.EnableObjectLocking);

            //Hard-code the LocationConstraint to the region from the config
            FastXmlWriter writer = new FastXmlWriter(128);
            writer.WriteStartElement("CreateBucketConfiguration");
            writer.WriteElement("LocationConstraint", ValueHelper.EnumToString(config.Region));
            writer.WriteEndElement("CreateBucketConfiguration");

            return new MemoryStream(writer.GetBytes());
        }
    }
}