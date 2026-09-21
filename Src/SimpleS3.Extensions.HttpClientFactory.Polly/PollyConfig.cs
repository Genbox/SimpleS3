using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;

/// <summary>Provides the <c>PollyConfig</c> API.</summary>
[PublicAPI]
public class PollyConfig
{
    // Default policy:
    // - Retries: 3
    // - Retry delay: 2^attempt seconds (2, 4, 8 seconds) + 0 to 1000 ms jitter
    // - Timeout per HTTP attempt: 10 minutes, through receipt of response headers
    // - Memory buffering threshold: 8 MiB

    /// <summary>Provides the <c>Retries</c> API.</summary>
    public int Retries { get; set; } = 3;

    /// <summary>Provides the <c>RetryMode</c> API.</summary>
    public RetryMode RetryMode { get; set; } = RetryMode.ExponentialBackoffJitter;

    /// <summary>Provides the <c>MaxRandomDelay</c> API.</summary>
    public TimeSpan MaxRandomDelay { get; set; } = TimeSpan.FromMilliseconds(1000);

    /// <summary>Timeout for each HTTP send attempt through receipt of response headers.</summary>
    /// <remarks>Retry delays, later response-body reads, XML parsing, and embedded S3 errors are outside this timeout. This is not an overall operation deadline.</remarks>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>Memory threshold for buffering forward-only request content before spilling to a temporary file.</summary>
    /// <remarks>The source is fully buffered before its first replayable read or Length access. This threshold does not limit total upload size or total process memory.</remarks>
    public int MaxMemoryBufferSize { get; set; } = 1024 * 1024 * 8;
}