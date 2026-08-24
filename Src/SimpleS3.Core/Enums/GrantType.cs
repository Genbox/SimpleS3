using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>GrantType</c> API.</summary>
[FastEnum]
public enum GrantType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Group</c> API.</summary>
    Group,
    /// <summary>Provides the <c>CanonicalUser</c> API.</summary>
    CanonicalUser,
    /// <summary>Provides the <c>AmazonCustomerByEmail</c> API.</summary>
    AmazonCustomerByEmail
}