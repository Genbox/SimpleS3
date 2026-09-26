using Genbox.SimpleS3.Core.Common.Marshal;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Bucket</c> API.</summary>
[PublicAPI]
public class S3Bucket : IHasBucketName
{
    internal S3Bucket(string bucketName, DateTimeOffset createdOn, string? bucketRegion = null, string? bucketArn = null)
    {
        CreatedOn = createdOn;
        BucketRegion = bucketRegion;
        BucketArn = bucketArn;
        BucketName = bucketName;
    }

    /// <summary>The date the bucket was created</summary>
    public DateTimeOffset CreatedOn { get; }

    /// <summary>The AWS region containing the bucket when returned by a paginated listing.</summary>
    public string? BucketRegion { get; }

    /// <summary>The Amazon Resource Name of the bucket when supplied by the service.</summary>
    public string? BucketArn { get; }

    /// <summary>Name of the bucket</summary>
    public string BucketName { get; }

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => BucketName;
}