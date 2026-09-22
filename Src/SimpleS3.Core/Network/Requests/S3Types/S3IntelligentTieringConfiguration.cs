namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>An Amazon S3 Intelligent-Tiering configuration.</summary>
public sealed class S3IntelligentTieringConfiguration
{
    /// <summary>Creates an Intelligent-Tiering configuration.</summary>
    public S3IntelligentTieringConfiguration(string id, bool enabled)
    {
        Id = id;
        Enabled = enabled;
    }

    /// <summary>Configuration identifier.</summary>
    public string Id { get; set; }

    /// <summary>Whether the configuration is enabled.</summary>
    public bool Enabled { get; set; }

    /// <summary>Optional object filter.</summary>
    public S3Filter? Filter { get; set; }

    /// <summary>Archive tier settings.</summary>
    public IList<S3IntelligentTieringTransition> Tierings { get; } = new List<S3IntelligentTieringTransition>();
}

/// <summary>An archive tier transition in an Intelligent-Tiering configuration.</summary>
public sealed class S3IntelligentTieringTransition
{
    /// <summary>Creates a tier transition.</summary>
    public S3IntelligentTieringTransition(int days, string accessTier)
    {
        Days = days;
        AccessTier = accessTier;
    }

    /// <summary>Number of inactive days before transition.</summary>
    public int Days { get; set; }

    /// <summary>Target access tier, such as <c>ARCHIVE_ACCESS</c> or <c>DEEP_ARCHIVE_ACCESS</c>.</summary>
    public string AccessTier { get; set; }
}