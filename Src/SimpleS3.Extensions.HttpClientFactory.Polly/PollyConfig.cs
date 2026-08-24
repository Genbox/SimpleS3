using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;

/// <summary>Provides the <c>PollyConfig</c> API.</summary>
[PublicAPI]
public class PollyConfig
{
    // Default policy:
    // - Retries: 3
    // - Timeout: 2^attempt seconds (2, 4, 8 seconds) + 0 to 1000 ms jitter
    // - Max memory usage: 8 MiB

    /// <summary>Provides the <c>Retries</c> API.</summary>
    public int Retries { get; set; } = 3;

    /// <summary>Provides the <c>RetryMode</c> API.</summary>
    public RetryMode RetryMode { get; set; } = RetryMode.ExponentialBackoffJitter;
    /// <summary>Provides the <c>MaxRandomDelay</c> API.</summary>
    public TimeSpan MaxRandomDelay { get; set; } = TimeSpan.FromMilliseconds(1000);
    /// <summary>Provides the <c>Timeout</c> API.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(10);
    /// <summary>Provides the <c>MaxMemoryBufferSize</c> API.</summary>
    public int MaxMemoryBufferSize { get; set; } = 1024 * 1024 * 8;
}