using Genbox.SimpleS3.Core.Common.Marshal;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Bucket</c> API.</summary>
[PublicAPI]
public class S3Bucket(string bucketName, DateTimeOffset createdOn) : IHasBucketName
{
    /// <summary>The date the bucket was created</summary>
    public DateTimeOffset CreatedOn { get; } = createdOn;

    /// <summary>Name of the bucket</summary>
    public string BucketName { get; } = bucketName;

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => BucketName;
}