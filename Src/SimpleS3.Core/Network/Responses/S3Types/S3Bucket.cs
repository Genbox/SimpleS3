using Genbox.SimpleS3.Core.Common.Marshal;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

[PublicAPI]
public class S3Bucket(string bucketName, DateTimeOffset createdOn) : IHasBucketName
{
    /// <summary>The date the bucket was created</summary>
    public DateTimeOffset CreatedOn { get; } = createdOn;

    /// <summary>Name of the bucket</summary>
    public string BucketName { get; } = bucketName;

    public override string ToString() => BucketName;
}