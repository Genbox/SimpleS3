using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Bucket : IHasBucketName
{
    public S3Bucket(string bucketName, DateTimeOffset createdOn)
    {
        BucketName = bucketName;
        CreatedOn = createdOn;
    }

    /// <summary>Name of the bucket</summary>
    public string BucketName { get; }

    /// <summary>The date the bucket was created</summary>
    public DateTimeOffset CreatedOn { get; }

    public override string ToString()
    {
        return BucketName;
    }
}