namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal static class SigningConstants
{
    public const string Scheme = "AWS4";
    public const string AlgorithmTag = Scheme + "-" + Algorithm;
    public const string ChunkedAlgorithmTag = AlgorithmTag + "-PAYLOAD";
    public const string TrailerAlgorithmTag = AlgorithmTag + "-TRAILER";
    public const string StreamingPayload = "STREAMING-AWS4-HMAC-SHA256-PAYLOAD";
    public const string StreamingPayloadTrailer = StreamingPayload + "-TRAILER";
    public const string AmazonHeaderPrefix = "x-amz-";
    public const string TrailerSignatureHeader = "x-amz-trailer-signature";
    public const string Algorithm = "HMAC-SHA256";
    public const char Colon = ':';
    public const char SemiColon = ';';
    public const char Newline = '\n';
}