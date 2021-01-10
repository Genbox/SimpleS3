namespace Genbox.SimpleS3.Core.Internals.Authentication
{
    internal static class SigningConstants
    {
        public const string Scheme = "AWS4";
        public const string AlgorithmTag = Scheme + "-" + Algorithm;
        public const string ChunkedAlgorithmTag = AlgorithmTag + "-PAYLOAD";
        public const string AmazonHeaderPrefix = "x-amz-";
        public const string Algorithm = "HMAC-SHA256";
        public const char Colon = ':';
        public const char SemiColon = ';';
        public const char Newline = '\n';
    }
}