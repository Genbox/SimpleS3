using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public class GetBucketLifecycleConfigurationResponse : BaseResponse
{
    public IList<S3Rule> Rules { get; } = new List<S3Rule>();
}