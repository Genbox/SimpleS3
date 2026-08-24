using Microsoft.Extensions.Logging;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal sealed partial class ChunkedSignatureBuilder
{
    private readonly ILogger<ChunkedSignatureBuilder> _logger = logger;

    [LoggerMessage(LogLevel.Trace, "Creating chunk signature for {RequestId}")]
    private partial void CreatingChunkSignature(Guid requestId);

    [LoggerMessage(LogLevel.Debug, "Chunk signature: {Signature}")]
    private partial void LogChunkSignature(byte[] signature);

    [LoggerMessage(LogLevel.Debug, "Chunk signature created")]
    private partial void ChunkSignatureCreated();

    [LoggerMessage(LogLevel.Trace, "Creating trailer signature for {RequestId}")]
    private partial void CreatingTrailerSignature(Guid requestId);

    [LoggerMessage(LogLevel.Debug, "Trailer signature: {Signature}")]
    private partial void LogTrailerSignature(byte[] signature);

    [LoggerMessage(LogLevel.Debug, "Trailer signature created")]
    private partial void TrailerSignatureCreated();

    [LoggerMessage(LogLevel.Trace, "Creating chunked StringToSign")]
    private partial void CreatingChunkedStringToSign();

    [LoggerMessage(LogLevel.Debug, "Chunked StringToSign: {StringToSign}")]
    private partial void LogChunkedStringToSign(string stringToSign);

    [LoggerMessage(LogLevel.Trace, "Creating trailer StringToSign")]
    private partial void CreatingTrailerStringToSign();

    [LoggerMessage(LogLevel.Debug, "Trailer StringToSign: {StringToSign}")]
    private partial void LogTrailerStringToSign(string stringToSign);
}