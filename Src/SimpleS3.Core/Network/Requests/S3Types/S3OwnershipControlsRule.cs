using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3OwnershipControlsRule
{
    public S3OwnershipControlsRule() {}

    public S3OwnershipControlsRule(ObjectOwnership objectOwnership)
    {
        ObjectOwnership = objectOwnership;
    }

    public ObjectOwnership ObjectOwnership { get; set; }
}