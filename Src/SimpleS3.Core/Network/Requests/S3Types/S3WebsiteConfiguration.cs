namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3WebsiteConfiguration</c> API.</summary>
public sealed class S3WebsiteConfiguration
{
    /// <summary>Provides the <c>IndexDocumentSuffix</c> API.</summary>
    public string? IndexDocumentSuffix { get; set; }

    /// <summary>Provides the <c>ErrorDocumentKey</c> API.</summary>
    public string? ErrorDocumentKey { get; set; }

    /// <summary>Provides the <c>RedirectAllRequestsTo</c> API.</summary>
    public S3RedirectAllRequestsTo? RedirectAllRequestsTo { get; set; }

    /// <summary>Provides the <c>RoutingRules</c> API.</summary>
    public IList<S3RoutingRule> RoutingRules { get; } = new List<S3RoutingRule>();
}