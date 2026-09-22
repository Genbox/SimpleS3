namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>An Amazon S3 metrics configuration.</summary>
public sealed class S3MetricsConfiguration
{
    /// <summary>Creates a metrics configuration.</summary>
    public S3MetricsConfiguration(string id)
    {
        Id = id;
    }

    /// <summary>Configuration identifier.</summary>
    public string Id { get; set; }

    /// <summary>Optional object filter.</summary>
    public S3Filter? Filter { get; set; }
}