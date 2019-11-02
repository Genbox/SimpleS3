using Genbox.SimpleS3.Abstracts.Constants;

namespace Genbox.SimpleS3.Core.Internal.Constants
{
    internal static class ObjectParameters
    {
        private const string _responseHeader = "response-";

        public const string ResponseCacheControl = _responseHeader + HttpHeaders.CacheControl;
        public const string ResponseExpires = _responseHeader + HttpHeaders.Expires;
        public const string ResponseContentDisposition = _responseHeader + HttpHeaders.ContentDisposition;
        public const string ResponseContentEncoding = _responseHeader + HttpHeaders.ContentEncoding;
        public const string ResponseContentLanguage = _responseHeader + HttpHeaders.ContentLanguage;
        public const string ResponseContentType = _responseHeader + HttpHeaders.ContentType;
        public const string VersionId = "versionId";
        public const string EncodingType = "encoding-type";
        public const string Delete = "delete";
    }
}