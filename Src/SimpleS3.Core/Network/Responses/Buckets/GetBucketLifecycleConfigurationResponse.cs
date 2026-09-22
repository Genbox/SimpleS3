using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketLifecycleConfigurationResponse</c> API.</summary>
public class GetBucketLifecycleConfigurationResponse : BaseResponse
{
    /// <summary>The default minimum object size for lifecycle transitions. Preserve the GET value when replacing a configuration.</summary>
    public TransitionDefaultMinimumObjectSize TransitionDefaultMinimumObjectSize { get; internal set; }

    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IList<S3Rule> Rules { get; } = new List<S3Rule>();
}