using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Grant</c> API.</summary>
public class S3Grant(S3Grantee grantee, S3Permission permission)
{
    /// <summary>Provides the <c>Grantee</c> API.</summary>
    public S3Grantee Grantee { get; } = grantee;

    /// <summary>Provides the <c>Permission</c> API.</summary>
    public S3Permission Permission { get; } = permission;
}