namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3RoutingRuleCondition</c> API.</summary>
public sealed class S3RoutingRuleCondition
{
    /// <summary>Provides the <c>KeyPrefixEquals</c> API.</summary>
    public string? KeyPrefixEquals { get; set; }

    /// <summary>Provides the <c>HttpErrorCodeReturnedEquals</c> API.</summary>
    public string? HttpErrorCodeReturnedEquals { get; set; }
}