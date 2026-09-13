using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>PutBucketLifecycleConfigurationResponse</summary>
public class PutBucketLifecycleConfigurationResponse : BaseResponse
{
    /// <summary>The default minimum object size reported for lifecycle transitions.</summary>
    public TransitionDefaultMinimumObjectSize TransitionDefaultMinimumObjectSize { get; internal set; }
}