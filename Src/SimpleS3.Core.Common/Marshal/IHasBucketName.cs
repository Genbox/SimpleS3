namespace Genbox.SimpleS3.Core.Common.Marshal;

/// <summary>Provides the <c>IHasBucketName</c> API.</summary>
public interface IHasBucketName
{
    /// <summary>Provides the <c>BucketName</c> API.</summary>
    string BucketName { get; }
}