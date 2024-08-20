using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Grantee(GrantType type, string? id, string? displayName, string? uri)
{
    public GrantType Type { get; } = type;
    public string? Id { get; } = id;
    public string? DisplayName { get; } = displayName;
    public string? Uri { get; } = uri;
}