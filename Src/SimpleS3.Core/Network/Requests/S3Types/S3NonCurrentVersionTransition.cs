using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3NonCurrentVersionTransition</c> API.</summary>
public class S3NonCurrentVersionTransition(int nonCurrentDays, StorageClass storageClass)
{
    /// <summary>Specifies the number of days an object is noncurrent before Amazon S3 can perform the associated action.</summary>
    public int NonCurrentDays { get; } = nonCurrentDays;

    /// <summary>The number of newer noncurrent versions to retain before transitioning an eligible version.</summary>
    public int? NewerNonCurrentVersions { get; set; }

    /// <summary>The class of storage used to store the object.</summary>
    public StorageClass StorageClass { get; } = storageClass;
}