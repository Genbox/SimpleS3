using System;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public class Config
    {
        public Config() { }

        public Config(IAccessKey credentials, string region)
        {
            Credentials = credentials;
            RegionCode = region;
        }

        /// <summary>The credentials to use when communicating with S3.</summary>
        public IAccessKey Credentials { get; set; }

        /// <summary>
        /// There are 3 different signing modes: 1. Unsigned - means the request will be sent without a signature at all. 2. FullSignature - Means the
        /// full payload will be hashed before sending. This option is better when you are only sending small objects (up to 32 MB). 3. StreamingSignature -
        /// Means the payload will be hashed in chunks and streamed to the server. This option is better when you also send large objects (up to 5 TB).
        /// </summary>
        public SignatureMode PayloadSignatureMode { get; set; } = SignatureMode.StreamingSignature;

        /// <summary>
        /// This is the number of bytes we read into memory, hash and send as a chunk to S3. Larger size means lower network overhead, but more memory
        /// usage.
        /// </summary>
        public int StreamingChunkSize { get; set; } = 2 * 1024 * 1024; // 2 Mb

        /// <summary>
        /// Controls if we use virtual hosts (bucketname.s3.eu-east-1.amazonaws.com) or sub-resources (s3.eu-east-1.amazonaws.com/bucketname) This
        /// setting only makes sense if you don't use a custom endpoint. See https://docs.aws.amazon.com/AmazonS3/latest/dev/VirtualHosting.html for more
        /// details.
        /// </summary>
        public NamingMode NamingMode { get; set; } = NamingMode.VirtualHost; //Amazon recommends virtual host. Path style urls was deprecated on 2020-09-30

        /// <summary>Set to true if you want to use an encrypted connection.</summary>
        public bool UseTls { get; set; } = true;

        /// <summary>Use this to set a custom endpoint. Note that <see cref="UseTls"/>, <see cref="RegionCode"/>, and <see cref="EndpointTemplate"/> will be ignored if you set an Endpoint manually</summary>
        public Uri? Endpoint { get; set; }

        /// <summary>
        /// You can either set an Endpoint manually or use the EndpointTemplate to have it calculated for you. If you set the Endpoint manually, you must hardcode the region to use (if using NamingMode.VirtualHost) or use NamingMode.PathStyle, where the bucket is inserted as part of the URL path.
        /// However, with EndpointTemplate, you can use NamingMode.VirtualHost and SimpleS3 will insert the bucket name (from each request) and region code (from config) into an URL before sending requests.
        /// </summary>
        public string? EndpointTemplate { get; set; }

        /// <summary>
        /// If enabled, bucket names are validated to ensure they are valid DNS names. This is to ensure you can always access your bucket using virtual
        /// host naming style.
        /// </summary>
        public bool EnableBucketNameValidation { get; set; }

        /// <summary>Controls the mode of validation that is applied to object keys. By default we only allow safe ASCII characters and a few special chars.</summary>
        public ObjectKeyValidationMode ObjectKeyValidationMode { get; set; } = ObjectKeyValidationMode.AsciiMode;

        /// <summary>If a response has EncodingType set to Url, SimpleS3 will automatically URL decode the encoded part of the response if this setting is true.</summary>
        public bool AutoUrlDecodeResponses { get; set; }

        /// <summary>When enabled, SimpleS3 will always calculate the ContentMD5 property before sending the request</summary>
        public bool AlwaysCalculateContentMd5 { get; set; }

        /// <summary>Set this to true to make SimpleS3 throw exceptions when it receives an error response from the S3 API.</summary>
        public bool ThrowExceptionOnError { get; set; }

        public string RegionCode { get; set; }

        public string ProviderName { get; set; }
    }
}