using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Gets the attribute-based access control status of a bucket.</summary>
public sealed class GetBucketAbacRequest : BaseRequest, IHasBucketName
{
    internal GetBucketAbacRequest() : base(HttpMethodType.GET) {}

    /// <summary>Creates a request.</summary>
    public GetBucketAbacRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Name of the bucket.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        BucketName = null!;
        base.Reset();
    }
}