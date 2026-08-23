using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class DeleteBucketIntelligentTieringConfigurationRequestMarshal : IRequestMarshal<DeleteBucketIntelligentTieringConfigurationRequest>
{
    public Stream? MarshalRequest(DeleteBucketIntelligentTieringConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.IntelligentTiering, string.Empty);
        request.SetQueryParameter(AmzParameters.Id, request.ConfigurationId);
        return null;
    }
}