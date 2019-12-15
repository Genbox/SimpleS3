namespace Genbox.SimpleS3.Core.Abstracts.Enums
{
    public enum ErrorCode
    {
        /// <summary>Access Denied</summary>
        AccessDenied,

        /// <summary>There is a problem with your AWS account that prevents the operation from completing successfully.</summary>
        AccountProblem,

        /// <summary>All access to this Amazon S3 resource has been disabled.</summary>
        AllAccessDisabled,

        /// <summary>The email address you provided is associated with more than one account.</summary>
        AmbiguousGrantByEmailAddress,

        /// <summary>The authorization header you provided is invalid.</summary>
        AuthorizationHeaderMalformed,

        /// <summary>The Content-MD5 you specified did not match what we received.</summary>
        BadDigest,

        /// <summary>
        /// The requested bucket name is not available. The bucket namespace is shared by all users of the system. Please select a different name and
        /// try again.
        /// </summary>
        BucketAlreadyExists,

        /// <summary>
        /// The bucket you tried to create already exists, and you own it. Amazon S3 returns this error in all AWS Regions except us-east-1 (N.
        /// Virginia). For legacy compatibility, if you re-create an existing bucket that you already own in us-east-1, Amazon S3 returns 200 OK and resets the
        /// bucket access control lists (ACLs).
        /// </summary>
        BucketAlreadyOwnedByYou,

        /// <summary>The bucket you tried to delete is not empty.</summary>
        BucketNotEmpty,

        /// <summary>This request does not support credentials.</summary>
        CredentialsNotSupported,

        /// <summary>Cross-location logging not allowed. Buckets in one geographic location cannot log information to a bucket in another location.</summary>
        CrossLocationLoggingProhibited,

        /// <summary>Your proposed upload is smaller than the minimum allowed object size.</summary>
        EntityTooSmall,

        /// <summary>Your proposed upload exceeds the maximum allowed object size.</summary>
        EntityTooLarge,

        /// <summary>The provided token has expired.</summary>
        ExpiredToken,

        /// <summary>Indicates that the versioning configuration specified in the request is invalid.</summary>
        IllegalVersioningConfigurationException,

        /// <summary>You did not provide the number of bytes specified by the Content-Length HTTP header</summary>
        IncompleteBody,

        /// <summary>POST requires exactly one file upload per request.</summary>
        IncorrectNumberOfFilesInPostRequest,

        /// <summary>Inline data exceeds the maximum allowed size.</summary>
        InlineDataTooLarge,

        /// <summary>We encountered an internal error. Please try again.</summary>
        InternalError,

        /// <summary>The AWS access key ID you provided does not exist in our records.</summary>
        InvalidAccessKeyId,

        /// <summary>You must specify the Anonymous role.</summary>
        InvalidAddressingHeader,

        /// <summary>Invalid Argument</summary>
        InvalidArgument,

        /// <summary>The specified bucket is not valid.</summary>
        InvalidBucketName,

        /// <summary>The request is not valid with the current state of the bucket.</summary>
        InvalidBucketState,

        /// <summary>The Content-MD5 you specified is not valid.</summary>
        InvalidDigest,

        /// <summary>The encryption request you specified is not valid. The valid value is AES256.</summary>
        InvalidEncryptionAlgorithmError,

        /// <summary>The specified location constraint is not valid.</summary>
        InvalidLocationConstraint,

        /// <summary>The operation is not valid for the current state of the object.</summary>
        InvalidObjectState,

        /// <summary>
        /// One or more of the specified parts could not be found. The part might not have been uploaded, or the specified entity tag might not have
        /// matched the part's entity tag.
        /// </summary>
        InvalidPart,

        /// <summary>The list of parts was not in ascending order. Parts list must be specified in order by part number.</summary>
        InvalidPartOrder,

        /// <summary>All access to this object has been disabled.</summary>
        InvalidPayer,

        /// <summary>The content of the form does not meet the conditions specified in the policy document.</summary>
        InvalidPolicyDocument,

        /// <summary>The requested range cannot be satisfied.</summary>
        InvalidRange,

        /// <summary>Invalid request</summary>
        InvalidRequest,

        /// <summary>The provided security credentials are not valid.</summary>
        InvalidSecurity,

        /// <summary>The SOAP request body is invalid.</summary>
        InvalidSOAPRequest,

        /// <summary>The storage class you specified is not valid.</summary>
        InvalidStorageClass,

        /// <summary>The target bucket for logging does not exist, is not owned by you, or does not have the appropriate grants for the log-delivery group.</summary>
        InvalidTargetBucketForLogging,

        /// <summary>The provided token is malformed or otherwise invalid.</summary>
        InvalidToken,

        /// <summary>Couldn't parse the specified URI.</summary>
        InvalidURI,

        /// <summary>Your key is too long.</summary>
        KeyTooLongError,

        /// <summary>The XML you provided was not well-formed or did not validate against our published schema.</summary>
        MalformedACLError,

        /// <summary>The body of your POST request is not well-formed multipart/form-data.</summary>
        MalformedPOSTRequest,

        /// <summary>
        /// This happens when the user sends malformed XML (XML that doesn't conform to the published XSD) for the configuration. The error message is,
        /// "The XML you provided was not well-formed or did not validate against our published schema."
        /// </summary>
        MalformedXML,

        /// <summary>Your request was too big.</summary>
        MaxMessageLengthExceeded,

        /// <summary>Your POST request fields preceding the upload file were too large.</summary>
        MaxPostPreDataLengthExceededError,

        /// <summary>Your metadata headers exceed the maximum allowed metadata size.</summary>
        MetadataTooLarge,

        /// <summary>The specified method is not allowed against this resource.</summary>
        MethodNotAllowed,

        /// <summary>A SOAP attachment was expected, but none were found.</summary>
        MissingAttachment,

        /// <summary>You must provide the Content-Length HTTP header.</summary>
        MissingContentLength,

        /// <summary>This happens when the user sends an empty XML document as a request. The error message is, "Request body is empty."</summary>
        MissingRequestBodyError,

        /// <summary>The SOAP 1.1 request is missing a security element.</summary>
        MissingSecurityElement,

        /// <summary>Your request is missing a required header.</summary>
        MissingSecurityHeader,

        /// <summary>There is no such thing as a logging status subresource for a key.</summary>
        NoLoggingStatusForKey,

        /// <summary>The specified bucket does not exist.</summary>
        NoSuchBucket,

        /// <summary>The specified bucket does not have a bucket policy.</summary>
        NoSuchBucketPolicy,

        /// <summary>The specified key does not exist.</summary>
        NoSuchKey,

        /// <summary>The lifecycle configuration does not exist.</summary>
        NoSuchLifecycleConfiguration,

        /// <summary>The specified multipart upload does not exist. The upload ID might be invalid, or the multipart upload might have been aborted or completed.</summary>
        NoSuchUpload,

        /// <summary>Indicates that the version ID specified in the request does not match an existing version.</summary>
        NoSuchVersion,

        /// <summary>A header you provided implies functionality that is not implemented.</summary>
        NotImplemented,

        /// <summary>Your account is not signed up for the Amazon S3 service.</summary>
        NotSignedUp,

        /// <summary>A conflicting conditional operation is currently in progress against this resource. Try again.</summary>
        OperationAborted,

        /// <summary>The bucket you are attempting to access must be addressed using the specified endpoint. Send all future requests to this endpoint.</summary>
        PermanentRedirect,

        /// <summary>At least one of the preconditions you specified did not hold.</summary>
        PreconditionFailed,

        /// <summary>Temporary redirect.</summary>
        Redirect,

        /// <summary>Object restore is already in progress.</summary>
        RestoreAlreadyInProgress,

        /// <summary>Bucket POST must be of the enclosure-type multipart/form-data.</summary>
        RequestIsNotMultiPartContent,

        /// <summary>Your socket connection to the server was not read from or written to within the timeout period.</summary>
        RequestTimeout,

        /// <summary>The difference between the request time and the server's time is too large.</summary>
        RequestTimeTooSkewed,

        /// <summary>Requesting the torrent file of a bucket is not permitted.</summary>
        RequestTorrentOfBucketError,

        /// <summary>The server side encryption configuration was not found.</summary>
        ServerSideEncryptionConfigurationNotFoundError,

        /// <summary>Reduce your request rate.</summary>
        ServiceUnavailable,

        /// <summary>The request signature we calculated does not match the signature you provided.</summary>
        SignatureDoesNotMatch,

        /// <summary>Reduce your request rate.</summary>
        SlowDown,

        /// <summary>You are being redirected to the bucket while DNS updates.</summary>
        TemporaryRedirect,

        /// <summary>The provided token must be refreshed.</summary>
        TokenRefreshRequired,

        /// <summary>You have attempted to create more buckets than allowed.</summary>
        TooManyBuckets,

        /// <summary>This request does not support content.</summary>
        UnexpectedContent,

        /// <summary>The email address you provided does not match any account on record.</summary>
        UnresolvableGrantByEmailAddress,

        /// <summary>The bucket POST must contain the specified field name. If it is specified, check the order of the fields.</summary>
        UserKeyMustBeSpecified
    }
}