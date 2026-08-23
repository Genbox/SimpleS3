using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketCorsRequestMarshal : IRequestMarshal<PutBucketCorsRequest>
{
    public Stream MarshalRequest(PutBucketCorsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Cors, string.Empty);
        FastXmlWriter writer = new FastXmlWriter(512);
        writer.WriteStartElement("CORSConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        foreach (S3CorsRule rule in request.Rules)
        {
            writer.WriteStartElement("CORSRule");
            if (rule.Id != null)
                writer.WriteElement("ID", rule.Id);
            foreach (string value in rule.AllowedHeaders)
                writer.WriteElement("AllowedHeader", value);
            foreach (string value in rule.AllowedMethods)
                writer.WriteElement("AllowedMethod", value);
            foreach (string value in rule.AllowedOrigins)
                writer.WriteElement("AllowedOrigin", value);
            foreach (string value in rule.ExposeHeaders)
                writer.WriteElement("ExposeHeader", value);
            if (rule.MaxAgeSeconds.HasValue)
                writer.WriteElement("MaxAgeSeconds", ValueHelper.IntToString(rule.MaxAgeSeconds.Value));
            writer.WriteEndElement("CORSRule");
        }
        writer.WriteEndElement("CORSConfiguration");
        return new MemoryStream(writer.GetBytes());
    }
}