using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Grant(S3Grantee grantee, S3Permission permission)
{
    public S3Grantee Grantee { get; } = grantee;
    public S3Permission Permission { get; } = permission;
}