namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>An Amazon S3 analytics configuration.</summary>
public sealed class S3AnalyticsConfiguration
{
    /// <summary>Creates an analytics configuration.</summary>
    public S3AnalyticsConfiguration(string id)
    {
        Id = id;
    }

    /// <summary>Configuration identifier.</summary>
    public string Id { get; set; }

    /// <summary>Optional object filter.</summary>
    public S3Filter? Filter { get; set; }

    /// <summary>Optional destination for storage-class analysis data.</summary>
    public S3AnalyticsDataExport? DataExport { get; set; }
}

/// <summary>Storage-class analysis export settings.</summary>
public sealed class S3AnalyticsDataExport
{
    /// <summary>Schema version. Amazon S3 currently supports <c>V_1</c>.</summary>
    public string OutputSchemaVersion { get; set; } = "V_1";

    /// <summary>Destination bucket ARN.</summary>
    public string BucketArn { get; set; } = null!;

    /// <summary>Optional destination bucket owner account ID.</summary>
    public string? BucketAccountId { get; set; }

    /// <summary>Optional object key prefix for generated reports.</summary>
    public string? Prefix { get; set; }

    /// <summary>Export format. Amazon S3 currently supports <c>CSV</c>.</summary>
    public string Format { get; set; } = "CSV";
}