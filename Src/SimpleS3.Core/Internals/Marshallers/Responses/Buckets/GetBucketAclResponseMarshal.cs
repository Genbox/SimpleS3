using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketAclResponseMarshal : IResponseMarshal<GetBucketAclResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketAclResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);
        (S3Identity? owner, List<S3Grant> grants) = ParserHelper.ParseAccessControlPolicy(xmlReader);
        response.Owner = owner;

        foreach (S3Grant grant in grants)
            response.Grants.Add(grant);
    }
}