namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Describes the restoration status of an archived object.</summary>
public sealed class S3RestoreStatus
{
    /// <summary>Whether restoration is currently in progress.</summary>
    public bool? IsRestoreInProgress { get; internal set; }

    /// <summary>When the restored copy expires.</summary>
    public DateTimeOffset? RestoreExpiryDate { get; internal set; }
}