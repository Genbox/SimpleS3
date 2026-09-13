namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>S3LoggingObjectKeyFormat</c> API.</summary>
public enum S3LoggingObjectKeyFormat
{
    /// <summary>No object-key format was supplied or reported.</summary>
    Unknown = 0,

    /// <summary>Provides the <c>SimplePrefix</c> API.</summary>
    SimplePrefix,
    /// <summary>Provides the <c>PartitionedPrefixEventTime</c> API.</summary>
    PartitionedPrefixEventTime,
    /// <summary>Provides the <c>PartitionedPrefixDeliveryTime</c> API.</summary>
    PartitionedPrefixDeliveryTime
}