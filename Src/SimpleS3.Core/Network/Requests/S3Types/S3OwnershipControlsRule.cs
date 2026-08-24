using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3OwnershipControlsRule</c> API.</summary>
public sealed class S3OwnershipControlsRule
{
    /// <summary>Provides the <c>S3OwnershipControlsRule</c> API.</summary>
    public S3OwnershipControlsRule() {}

    /// <summary>Provides the <c>S3OwnershipControlsRule</c> API.</summary>
    public S3OwnershipControlsRule(ObjectOwnership objectOwnership)
    {
        ObjectOwnership = objectOwnership;
    }

    /// <summary>Provides the <c>ObjectOwnership</c> API.</summary>
    public ObjectOwnership ObjectOwnership { get; set; }
}