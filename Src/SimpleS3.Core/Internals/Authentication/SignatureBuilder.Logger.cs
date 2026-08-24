using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal sealed partial class SignatureBuilder
{
    private readonly ILogger<SignatureBuilder> _logger = logger;

    [LoggerMessage(LogLevel.Trace, "Creating signature for {RequestId}")]
    private partial void CreatingSignature(Guid requestId);

    [LoggerMessage(LogLevel.Debug, "Signature: {Signature}")]
    private partial void LogSignature(byte[] signature);

    [LoggerMessage(LogLevel.Debug, "Signature created")]
    private partial void SignatureCreated();

    [LoggerMessage(LogLevel.Trace, "Creating canonical request for {RequestId}")]
    private partial void CreatingCanonicalRequest(Guid requestId);

    [LoggerMessage(LogLevel.Debug, "CanonicalRequest: {CanonicalRequest}")]
    private partial void LogCanonicalRequest(string canonicalRequest);

    [LoggerMessage(LogLevel.Trace, "Creating StringToSign")]
    private partial void CreatingStringToSign();

    [LoggerMessage(LogLevel.Debug, "StringToSign: {StringToSign}")]
    private partial void LogStringToSign(string stringToSign);
}