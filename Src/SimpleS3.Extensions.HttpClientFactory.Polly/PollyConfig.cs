using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;

[PublicAPI]
public class PollyConfig
{
    //Default policy:
    //- Retries: 3
    //-Timeout: 2^attempt seconds (2, 4, 8 seconds) + 0 to 1000 ms jitter

    public int Retries { get; set; } = 3;

    public RetryMode RetryMode { get; set; } = RetryMode.ExponentialBackoffJitter;
    public TimeSpan MaxRandomDelay { get; set; } = TimeSpan.FromMilliseconds(1000);
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(10);
}