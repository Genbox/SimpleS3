using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum RequestPaymentPayer
{
    Unknown = 0,
    Requester,
    BucketOwner
}