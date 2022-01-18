using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Grantee
{
    public S3Grantee(GrantType type, string? id, string? displayName, string? uri)
    {
        Type = type;
        Id = id;
        DisplayName = displayName;
        Uri = uri;
    }

    public GrantType Type { get; }
    public string? Id { get; }
    public string? DisplayName { get; }
    public string? Uri { get; }
}