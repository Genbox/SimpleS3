namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3RoutingRule</c> API.</summary>
public sealed class S3RoutingRule(S3Redirect redirect)
{
    /// <summary>Provides the <c>Condition</c> API.</summary>
    public S3RoutingRuleCondition? Condition { get; set; }
    /// <summary>Provides the <c>Redirect</c> API.</summary>
    public S3Redirect Redirect { get; set; } = redirect;
}