using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Grantee</c> API.</summary>
public class S3Grantee(GrantType type, string? id, string? displayName, string? uri)
{
    /// <summary>Provides the <c>Type</c> API.</summary>
    public GrantType Type { get; } = type;
    /// <summary>Provides the <c>Id</c> API.</summary>
    public string? Id { get; } = id;
    /// <summary>Provides the <c>DisplayName</c> API.</summary>
    public string? DisplayName { get; } = displayName;
    /// <summary>Provides the <c>Uri</c> API.</summary>
    public string? Uri { get; } = uri;
}