using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>GetBucketLocationRequest</c> API.</summary>
public sealed class GetBucketLocationRequest : BaseRequest, IHasBucketName
{
    internal GetBucketLocationRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetBucketLocationRequest</c> API.</summary>
    public GetBucketLocationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    internal override void Reset()
    {
        BucketName = null!;
        base.Reset();
    }
}