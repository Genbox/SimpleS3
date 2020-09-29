namespace Genbox.SimpleS3.Core.Abstracts.Constants
{
    public static class AmzParameters
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
        public const string Delimiter = "delimiter";
        public const string MaxKeys = "max-keys";
        public const string Prefix = "prefix";
        public const string ListType = "list-type";
        public const string ContinuationToken = "continuation-token";
        public const string FetchOwner = "fetch-owner";
        public const string StartAfter = "start-after";
        public const string MaxUploads = "max-uploads";
        public const string KeyMarker = "key-marker";
        public const string UploadIdMarker = "upload-id-marker";
        public const string MaxParts = "max-parts";
        public const string PartNumberMarker = "part-number-marker";
        public const string UploadId = "uploadId";
        public const string PartNumber = "partNumber";
        public const string Uploads = "uploads";
        public const string Restore = "restore";
        public const string Acl = "acl";
        public const string LegalHold = "legal-hold";
        public const string ObjectLock = "object-lock";
        public const string Tagging = "tagging";
        public const string Accelerate = "accelerate";
        public const string Lifecycle = "lifecycle";

        #region Presigning

        //Note: These are actually case-sensitive
        public const string XAmzAlgorithm = "X-Amz-Algorithm";
        public const string XAmzCredential = "X-Amz-Credential";
        public const string XAmzSignedHeaders = "X-Amz-SignedHeaders";
        public const string XAmzSignature = "X-Amz-Signature";
        public const string XAmzExpires = "X-Amz-Expires";
        public const string XAmzDate = "X-Amz-Date";

        #endregion
    }
}