using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class ListBucketMetricsConfigurationsResponseMarshal : IResponseMarshal<ListBucketMetricsConfigurationsResponse>
{
    public void MarshalResponse(SimpleS3Config config, ListBucketMetricsConfigurationsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.MoveToContent();
        response.IsTruncated = false;
        bool isTruncatedRead = false;
        bool continuationTokenRead = false;
        bool nextContinuationTokenRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "IsTruncated" when !isTruncatedRead:
                    response.IsTruncated = bool.TryParse(reader.ReadString(), out bool truncated) && truncated;
                    isTruncatedRead = true;
                    break;
                case "ContinuationToken" when !continuationTokenRead:
                    response.ContinuationToken = reader.ReadString();
                    continuationTokenRead = true;
                    break;
                case "NextContinuationToken" when !nextContinuationTokenRead:
                    response.NextContinuationToken = reader.ReadString();
                    nextContinuationTokenRead = true;
                    break;
                case "MetricsConfiguration":
                    response.Configurations.Add(MetricsConfigurationXml.Parse(reader));
                    break;
            }
        }
    }
}