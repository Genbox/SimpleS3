using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>S3 Metadata V2 configuration.</summary>
public sealed class S3MetadataConfiguration(S3JournalTableConfiguration journalTableConfiguration)
{
    /// <summary>Journal table configuration.</summary>
    public S3JournalTableConfiguration JournalTableConfiguration { get; set; } = journalTableConfiguration;

    /// <summary>Optional live inventory table configuration.</summary>
    public S3InventoryTableConfiguration? InventoryTableConfiguration { get; set; }

    /// <summary>Optional annotation table configuration.</summary>
    public S3AnnotationTableConfiguration? AnnotationTableConfiguration { get; set; }
}

/// <summary>Journal table configuration.</summary>
public sealed class S3JournalTableConfiguration(S3RecordExpiration recordExpiration)
{
    /// <summary>Journal record expiration configuration.</summary>
    public S3RecordExpiration RecordExpiration { get; set; } = recordExpiration;

    /// <summary>Optional encryption configuration.</summary>
    public S3MetadataTableEncryptionConfiguration? EncryptionConfiguration { get; set; }
}

/// <summary>Journal table record expiration configuration.</summary>
public sealed class S3RecordExpiration(MetadataTableConfigurationState expiration)
{
    /// <summary>Whether expiration is enabled.</summary>
    public MetadataTableConfigurationState Expiration { get; set; } = expiration;

    /// <summary>Number of days to retain records when enabled.</summary>
    public int? Days { get; set; }
}

/// <summary>Live inventory table configuration.</summary>
public sealed class S3InventoryTableConfiguration(MetadataTableConfigurationState configurationState)
{
    /// <summary>Whether the inventory table is enabled.</summary>
    public MetadataTableConfigurationState ConfigurationState { get; set; } = configurationState;

    /// <summary>Optional encryption configuration.</summary>
    public S3MetadataTableEncryptionConfiguration? EncryptionConfiguration { get; set; }
}