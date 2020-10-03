using System;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IConfig
    {
        /// <summary>The credentials to use when communicating with S3.</summary>
        IAccessKey Credentials { get; set; }

        /// <summary>This defines the region we use for communicating with S3. If you specify your own endpoint, this value is only used for signatures.</summary>
        AwsRegion Region { get; set; }

        /// <summary>
        /// There are 3 different signing modes: 1. Unsigned - means the request will be sent without a signature at all. 2. FullSignature - Means the
        /// full payload will be hashed before sending. This option is better when you are only sending small objects (up to 32 MB). 3. StreamingSignature -
        /// Means the payload will be hashed in chunks and streamed to the server. This option is better when you also send large objects (up to 5 TB).
        /// </summary>
        SignatureMode PayloadSignatureMode { get; set; }

        /// <summary>
        /// This is the number of bytes we read into memory, hash and send as a chunk to S3. Larger size means lower network overhead, but more memory
        /// usage.
        /// </summary>
        int StreamingChunkSize { get; set; }

        /// <summary>
        /// Controls if we use virtual hosts (bucketname.s3.eu-east-1.amazonaws.com) or sub-resources (s3.eu-east-1.amazonaws.com/bucketname) This
        /// setting only makes sense if you don't use a custom endpoint. See https://docs.aws.amazon.com/AmazonS3/latest/dev/VirtualHosting.html for more
        /// details.
        /// </summary>
        NamingMode NamingMode { get; set; }

        /// <summary>Set to true if you want to use an encrypted connection.</summary>
        bool UseTLS { get; set; }

        /// <summary>Use this to set a custom endpoint. For example, when using minio, you can set it to https://miniohost.com/</summary>
        Uri? Endpoint { get; set; }

        /// <summary>
        /// If enabled, bucket names are validated to ensure they are valid DNS names. This is to ensure you can always access your bucket using virtual
        /// host naming style.
        /// </summary>
        bool EnableBucketNameValidation { get; set; }

        /// <summary>Controls the mode of validation that is applied to object keys. By default we only allow safe ASCII characters and a few special chars.</summary>
        KeyValidationMode ObjectKeyValidationMode { get; set; }

        /// <summary>If a response has EncodingType set to Url, SimpleS3 will automatically URL decode the encoded part of the response if this setting is true.</summary>
        bool AutoUrlDecodeResponses { get; set; }

        /// <summary>When enabled, SimpleS3 will always calculate the ContentMD5 property before sending the request</summary>
        bool AlwaysCalculateContentMd5 { get; set; }

        /// <summary>Set this to true to make SimpleS3 throw exceptions when it receives an error response from the S3 API.</summary>
        bool ThrowExceptionOnError { get; set; }
    }
}