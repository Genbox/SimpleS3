namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasBucketKeyEnabled</c> API.</summary>
public interface IHasBucketKeyEnabled
{
    /// <summary>Whether Amazon S3 used an S3 Bucket Key for SSE-KMS.</summary>
    bool? BucketKeyEnabled { get; }
}