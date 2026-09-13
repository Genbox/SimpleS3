using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>GetBucketLoggingRequest</c> API.</summary>
public sealed class GetBucketLoggingRequest : BaseRequest, IHasBucketName
{
    internal GetBucketLoggingRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetBucketLoggingRequest</c> API.</summary>
    public GetBucketLoggingRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        BucketName = null!;
        base.Reset();
    }
}