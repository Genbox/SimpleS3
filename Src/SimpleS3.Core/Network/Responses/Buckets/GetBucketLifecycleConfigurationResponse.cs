using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketLifecycleConfigurationResponse</c> API.</summary>
public class GetBucketLifecycleConfigurationResponse : BaseResponse
{
    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IList<S3Rule> Rules { get; } = new List<S3Rule>();
}