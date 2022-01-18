using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal class PutBucketAccelerateConfigurationRequestMarshal : IRequestMarshal<PutBucketAccelerateConfigurationRequest>
{
    public Stream? MarshalRequest(PutBucketAccelerateConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Accelerate, string.Empty);

        FastXmlWriter writer = new FastXmlWriter(150);
        writer.WriteStartElement("AccelerateConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("Status", request.AccelerationEnabled ? "Enabled" : "Suspended");
        writer.WriteEndElement("AccelerateConfiguration");

        return new MemoryStream(writer.GetBytes());
    }
}