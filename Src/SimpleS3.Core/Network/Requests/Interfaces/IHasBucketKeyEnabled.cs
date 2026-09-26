namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

/// <summary>Provides the <c>IHasBucketKeyEnabled</c> API.</summary>
public interface IHasBucketKeyEnabled
{
    /// <summary>Specifies whether Amazon S3 should use an S3 Bucket Key for server-side encryption with AWS KMS.</summary>
    bool? BucketKeyEnabled { get; set; }
}