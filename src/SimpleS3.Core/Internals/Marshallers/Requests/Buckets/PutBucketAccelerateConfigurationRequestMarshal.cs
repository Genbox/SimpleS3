using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets
{
    [UsedImplicitly]
    internal class PutBucketAccelerateConfigurationRequestMarshal : IRequestMarshal<PutBucketAccelerateConfigurationRequest>
    {
        public Stream MarshalRequest(PutBucketAccelerateConfigurationRequest request, IConfig config)
        {
            request.SetQueryParameter(AmzParameters.Accelerate, string.Empty);

            FastXmlWriter writer = new FastXmlWriter(150);
            writer.WriteStartElement("AccelerateConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
            writer.WriteElement("Status", request.AccelerationEnabled ? "Enabled" : "Suspended");
            writer.WriteEndElement("AccelerateConfiguration");

            return new MemoryStream(writer.GetBytes());
        }
    }
}