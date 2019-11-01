using System;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IS3Config
    {
        /// <summary>The credentials to use when communicating with S3.</summary>
        IAccessKey Credentials { get; set; }

        /// <summary>This defines the region we use for communicating with S3. If you specify your own endpoint, this value is only used for signatures.</summary>
        AwsRegion Region { get; set; }

        /// <summary>This enables payload signing, which means we hash data before sending it to Amazon.</summary>
        bool EnablePayloadSigning { get; set; }

        /// <summary>
        /// Enables support for streaming signatures. When uploading data, Amazon S3 expects a hash of the data before the upload begins. This either
        /// means we have to load everything into memory to hash it, or fully read a stream twice. Both solutions result in bad performance. With streaming, we
        /// read a smaller chunk of the data, hash it and send it to amazon. This gives much better performance, with a small overhead of having to send
        /// streaming chunk headers.
        /// </summary>
        bool EnableStreaming { get; set; }

        /// <summary>
        /// This is the number of bytes we read into memory, hash and send as a chunk to S3. Larger size means lower network overhead, but more memory
        /// usage.
        /// </summary>
        int StreamingChunkSize { get; set; } //8 Mb

        /// <summary>
        /// Controls if we use virtual hosts (bucketname.s3.eu-east-1.amazonaws.com) or sub-resources (s3.eu-east-1.amazonaws.com/bucketname) This
        /// setting only makes sense if you don't use a custom endpoint. See https://docs.aws.amazon.com/AmazonS3/latest/dev/VirtualHosting.html for more
        /// details.
        /// </summary>
        NamingType NamingType { get; set; }

        /// <summary>Use this to set a custom endpoint. For example, when using minio, you can set it to https://miniohost.com/</summary>
        Uri Endpoint { get; set; }

        /// <summary>
        /// If enabled, bucket names are validated to ensure they are valid DNS names. This is to ensure you can always access your bucket using virtual
        /// host naming style.
        /// </summary>
        bool EnableBucketNameValidation { get; set; }

        /// <summary>
        /// Controls the mode of validation that is applied to object keys. By default we only allow safe ASCII characters and a few special chars.
        /// </summary>
        KeyValidationMode ObjectKeyValidationMode { get; set; }
    }
}