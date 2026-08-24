using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>RequestPaymentPayer</c> API.</summary>
[FastEnum]
public enum RequestPaymentPayer
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Requester</c> API.</summary>
    Requester,
    /// <summary>Provides the <c>BucketOwner</c> API.</summary>
    BucketOwner
}