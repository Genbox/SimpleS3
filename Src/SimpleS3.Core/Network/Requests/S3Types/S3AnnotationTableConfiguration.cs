using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3AnnotationTableConfiguration(AnnotationTableConfigurationState configurationState)
{
    public AnnotationTableConfigurationState ConfigurationState { get; set; } = configurationState;
    public S3MetadataTableEncryptionConfiguration? EncryptionConfiguration { get; set; }
    public string? Role { get; set; }
}