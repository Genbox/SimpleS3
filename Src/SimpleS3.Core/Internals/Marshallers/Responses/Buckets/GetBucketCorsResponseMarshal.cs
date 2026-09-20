using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketCorsResponseMarshal : IResponseMarshal<GetBucketCorsResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketCorsResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);

        while (reader.ReadToFollowing("CORSRule"))
        {
            S3CorsRule rule = new S3CorsRule();

            foreach (string name in XmlHelper.ReadElements(reader, "CORSRule"))
            {
                switch (name)
                {
                    case "ID":
                        rule.Id = reader.ReadString();
                        break;
                    case "AllowedHeader":
                        rule.AllowedHeaders.Add(reader.ReadString());
                        break;
                    case "AllowedMethod":
                        rule.AllowedMethods.Add(reader.ReadString());
                        break;
                    case "AllowedOrigin":
                        rule.AllowedOrigins.Add(reader.ReadString());
                        break;
                    case "ExposeHeader":
                        rule.ExposeHeaders.Add(reader.ReadString());
                        break;
                    case "MaxAgeSeconds":
                        rule.MaxAgeSeconds = ValueHelper.ParseInt(reader.ReadString());
                        break;
                }
            }

            response.Rules.Add(rule);
        }
    }
}