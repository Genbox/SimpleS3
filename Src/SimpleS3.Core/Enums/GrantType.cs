using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum GrantType
{
    Unknown = 0,
    Group,
    CanonicalUser,
    AmazonCustomerByEmail
}