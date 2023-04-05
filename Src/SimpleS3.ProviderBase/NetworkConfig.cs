using System.Net;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;

namespace Genbox.SimpleS3.ProviderBase;

public class NetworkConfig
{
    public IWebProxy? Proxy { get; set; }
    public int Retries { get; set; } = 3;
    public RetryMode RetryMode { get; set; } = RetryMode.ExponentialBackoffJitter;
    public TimeSpan MaxRandomDelay { get; set; } = TimeSpan.FromMilliseconds(1000);
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(10);
}