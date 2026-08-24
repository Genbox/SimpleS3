using System.Net;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;

namespace Genbox.SimpleS3.ProviderBase;

/// <summary>Provides the <c>NetworkConfig</c> API.</summary>
public class NetworkConfig
{
    /// <summary>Provides the <c>Proxy</c> API.</summary>
    public IWebProxy? Proxy { get; set; }
    /// <summary>Provides the <c>Retries</c> API.</summary>
    public int Retries { get; set; } = 3;
    /// <summary>Provides the <c>RetryMode</c> API.</summary>
    public RetryMode RetryMode { get; set; } = RetryMode.ExponentialBackoffJitter;
    /// <summary>Provides the <c>MaxRandomDelay</c> API.</summary>
    public TimeSpan MaxRandomDelay { get; set; } = TimeSpan.FromMilliseconds(1000);
    /// <summary>Provides the <c>Timeout</c> API.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(10);
}