using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketRequestPaymentRequestMarshal : IRequestMarshal<PutBucketRequestPaymentRequest>
{
    public Stream MarshalRequest(PutBucketRequestPaymentRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.RequestPayment, string.Empty);
        FastXmlWriter writer = new FastXmlWriter(128);
        writer.WriteStartElement("RequestPaymentConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        if (request.Payer != RequestPaymentPayer.Unknown)
            writer.WriteElement("Payer", request.Payer.GetString());
        writer.WriteEndElement("RequestPaymentConfiguration");
        return new MemoryStream(writer.GetBytes());
    }
}