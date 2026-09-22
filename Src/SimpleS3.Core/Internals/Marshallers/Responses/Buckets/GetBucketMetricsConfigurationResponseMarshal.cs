using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketMetricsConfigurationResponseMarshal : IResponseMarshal<GetBucketMetricsConfigurationResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketMetricsConfigurationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.MoveToContent();
        response.Configuration = MetricsConfigurationXml.Parse(reader);
    }
}