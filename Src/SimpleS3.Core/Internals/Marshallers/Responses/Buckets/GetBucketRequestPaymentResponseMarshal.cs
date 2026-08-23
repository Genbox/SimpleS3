using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketRequestPaymentResponseMarshal : IResponseMarshal<GetBucketRequestPaymentResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketRequestPaymentResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        if (reader.ReadToDescendant("Payer"))
            response.Payer = Core.Enums.Enums.RequestPaymentPayer.Parse(reader.ReadElementContentAsString());
    }
}