namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public sealed class S3RoutingRule(S3Redirect redirect)
{
    public S3RoutingRuleCondition? Condition { get; set; }
    public S3Redirect Redirect { get; set; } = redirect;
}