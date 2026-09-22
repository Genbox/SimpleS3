using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>An Amazon S3 inventory configuration.</summary>
public sealed class S3InventoryConfiguration
{
    /// <summary>Creates an inventory configuration.</summary>
    public S3InventoryConfiguration(string id)
    {
        Id = id;
    }

    /// <summary>Configuration identifier.</summary>
    public string Id { get; set; }

    /// <summary>Whether inventory generation is enabled.</summary>
    public bool Enabled { get; set; }

    /// <summary>Object versions included in reports. Amazon S3 supports <c>All</c> and <c>Current</c>.</summary>
    public string IncludedObjectVersions { get; set; } = "All";

    /// <summary>Optional object key prefix filter.</summary>
    public string? Prefix { get; set; }

    /// <summary>Optional object metadata fields included in reports.</summary>
    public IList<string> OptionalFields { get; } = new List<string>();

    /// <summary>Report frequency. Amazon S3 supports <c>Daily</c> and <c>Weekly</c>.</summary>
    public string ScheduleFrequency { get; set; } = "Daily";

    /// <summary>Destination for generated reports.</summary>
    public S3InventoryDestination Destination { get; set; } = null!;
}

/// <summary>S3 destination for inventory reports.</summary>
public sealed class S3InventoryDestination
{
    /// <summary>Optional destination bucket owner account ID.</summary>
    public string? AccountId { get; set; }

    /// <summary>Destination bucket ARN.</summary>
    public string BucketArn { get; set; } = null!;

    /// <summary>Report format. Amazon S3 supports <c>CSV</c>, <c>ORC</c>, and <c>Parquet</c>.</summary>
    public string Format { get; set; } = "CSV";

    /// <summary>Optional object key prefix for generated reports.</summary>
    public string? Prefix { get; set; }

    /// <summary>Optional server-side encryption configuration.</summary>
    public S3InventoryEncryption? Encryption { get; set; }
}

/// <summary>Server-side encryption settings for inventory reports.</summary>
public sealed class S3InventoryEncryption
{
    /// <summary>Encryption type.</summary>
    public S3InventoryEncryptionType Type { get; set; }

    /// <summary>AWS KMS key ID or ARN when <see cref="Type" /> is <see cref="S3InventoryEncryptionType.SseKms" />.</summary>
    public string? KeyId { get; set; }
}