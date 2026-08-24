using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>GetBucketLocationRequest</c> API.</summary>
public sealed class GetBucketLocationRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner
{
    internal GetBucketLocationRequest() : base(HttpMethodType.GET) {}
    /// <summary>Provides the <c>GetBucketLocationRequest</c> API.</summary>
    public GetBucketLocationRequest(string bucketName) : this() => Initialize(bucketName);
    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>ExpectedBucketOwner</c> API.</summary>
    public string? ExpectedBucketOwner { get; set; }
    internal void Initialize(string bucketName) => BucketName = bucketName;
    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset() { ExpectedBucketOwner = null; base.Reset(); }
}