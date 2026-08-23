namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3RoutingRuleCondition
{
    public string? KeyPrefixEquals { get; set; }
    public string? HttpErrorCodeReturnedEquals { get; set; }
}