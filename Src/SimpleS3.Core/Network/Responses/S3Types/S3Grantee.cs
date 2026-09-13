using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Grantee</c> API.</summary>
[SuppressMessage("Design", "CA1054", Justification = "S3 group URIs are protocol identifiers whose exact string representation is preserved in ACLs.")]
public class S3Grantee(GrantType type, string? id, string? displayName, string? uri, string? emailAddress = null)
{
    /// <summary>Provides the <c>Type</c> API.</summary>
    public GrantType Type { get; } = type;

    /// <summary>Provides the <c>Id</c> API.</summary>
    public string? Id { get; } = id;

    /// <summary>Provides the <c>DisplayName</c> API.</summary>
    public string? DisplayName { get; } = displayName;

    /// <summary>Provides the <c>Uri</c> API.</summary>
    [SuppressMessage("Design", "CA1056", Justification = "The value is an S3 group identifier preserved verbatim from the ACL response.")]
    public string? Uri { get; } = uri;

    /// <summary>Provides the <c>EmailAddress</c> API.</summary>
    public string? EmailAddress { get; } = emailAddress;
}