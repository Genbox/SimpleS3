using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3AnnotationTableConfiguration</c> API.</summary>
public sealed class S3AnnotationTableConfiguration(AnnotationTableConfigurationState configurationState)
{
    /// <summary>Provides the <c>ConfigurationState</c> API.</summary>
    public AnnotationTableConfigurationState ConfigurationState { get; set; } = configurationState;

    /// <summary>Provides the <c>EncryptionConfiguration</c> API.</summary>
    public S3MetadataTableEncryptionConfiguration? EncryptionConfiguration { get; set; }

    /// <summary>Provides the <c>Role</c> API.</summary>
    public string? Role { get; set; }
}