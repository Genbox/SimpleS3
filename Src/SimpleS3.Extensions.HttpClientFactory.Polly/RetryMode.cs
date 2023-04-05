namespace Genbox.SimpleS3.Extensions.HttpClientFactory.Polly;

public enum RetryMode
{
    NoDelay = 0,
    LinearDelay,
    LinearDelayJitter,
    ExponentialBackoff,
    ExponentialBackoffJitter
}