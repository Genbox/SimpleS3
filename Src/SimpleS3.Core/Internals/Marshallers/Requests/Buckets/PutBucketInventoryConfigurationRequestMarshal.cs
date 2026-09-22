using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketInventoryConfigurationRequestMarshal : IRequestMarshal<PutBucketInventoryConfigurationRequest>
{
    public Stream MarshalRequest(PutBucketInventoryConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Inventory, string.Empty);
        request.SetQueryParameter(AmzParameters.Id, request.ConfigurationId);
        FastXmlWriter writer = new FastXmlWriter(500);
        InventoryConfigurationXml.Write(writer, request.Configuration);
        return new MemoryStream(writer.GetBytes());
    }
}