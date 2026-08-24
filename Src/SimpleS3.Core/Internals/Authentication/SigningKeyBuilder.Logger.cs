using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal sealed partial class SigningKeyBuilder
{
    private readonly ILogger<SigningKeyBuilder> _logger;

    [LoggerMessage(LogLevel.Trace, "Creating signing key for {DateTime}")]
    private partial void CreatingSigningKey(DateTimeOffset dateTime);

    [LoggerMessage(LogLevel.Debug, "Signing key created: {SigningKey}")]
    private partial void LogSigningKey(byte[] signingKey);

    [LoggerMessage(LogLevel.Debug, "Signing key created")]
    private partial void SigningKeyCreated();
}