namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;

/// <summary>Provides the <c>RetryMode</c> API.</summary>
public enum RetryMode
{
    /// <summary>Provides the <c>NoDelay</c> API.</summary>
    NoDelay = 0,
    /// <summary>Provides the <c>LinearDelay</c> API.</summary>
    LinearDelay,
    /// <summary>Provides the <c>LinearDelayJitter</c> API.</summary>
    LinearDelayJitter,
    /// <summary>Provides the <c>ExponentialBackoff</c> API.</summary>
    ExponentialBackoff,
    /// <summary>Provides the <c>ExponentialBackoffJitter</c> API.</summary>
    ExponentialBackoffJitter
}