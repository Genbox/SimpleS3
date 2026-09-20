using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts;

/// <summary>Provides the <c>SimpleS3Config</c> API.</summary>
[PublicAPI]
public class SimpleS3Config
{
    /// <summary>Provides the <c>SimpleS3Config</c> API.</summary>
    public SimpleS3Config() {} //Needed for config binding

    /// <summary>Provides the <c>SimpleS3Config</c> API.</summary>
    public SimpleS3Config(string providerName, string endpoint)
    {
        ProviderName = providerName;
        Endpoint = endpoint;
    }

    /// <summary>Provides the <c>SimpleS3Config</c> API.</summary>
    public SimpleS3Config(IAccessKey? credentials, string endpoint, string regionCode) : this(string.Empty, endpoint) //Used internally
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    /// <summary>The credentials to use when communicating with S3.</summary>
    public IAccessKey? Credentials { get; set; }

    /// <summary>
    /// There are 3 different signing modes: 1. Unsigned - the payload is not signed; requests still use authentication when credentials are configured. 2. FullSignature - Means the full payload will be hashed
    /// before sending. This option is better when you are only sending small objects (up to 32 MB). 3. StreamingSignature - Means the payload will be hashed in chunks and streamed to the
    /// server. This option is better when you also send large objects (up to 5 TB).
    /// </summary>
    public SignatureMode PayloadSignatureMode { get; set; } = SignatureMode.StreamingSignature;

    /// <summary>This is the number of bytes we read into memory, hash and send as a chunk to S3. Larger size means lower network overhead, but more memory usage.</summary>
    public int StreamingChunkSize { get; set; } = 80 * 1024; // 80 KB

    /// <summary>
    /// Controls if we use virtual hosts (bucketname.s3.eu-east-1.amazonaws.com) or sub-resources (s3.eu-east-1.amazonaws.com/bucketname) This setting only makes sense if you
    /// don't use a custom endpoint. See https://docs.aws.amazon.com/AmazonS3/latest/dev/VirtualHosting.html for more details.
    /// </summary>
    public NamingMode NamingMode { get; set; } = NamingMode.VirtualHost; //Amazon recommends virtual host. Path style urls was deprecated on 2020-09-30

    /// <summary>Set to true if you want to use an encrypted connection.</summary>
    public bool UseTls { get; set; } = true;

    /// <summary>
    ///     <para>
    ///     Use this to set a custom endpoint. It can either be a URL like https://myserver.com:9101 or a template. A literal URL keeps its supplied scheme and host.
    ///     Path-style naming still adds the bucket to the request path; virtual-host naming uses the supplied host as-is. RegionCode still determines the signing region.
    ///     </para>
    ///     <para>
    ///     However, you can also set the endpoint to a template in the form of {Scheme}{Region:.}myserver.com:9101/{Bucket} and SimpleS3 will build an URL for you using
    ///     <see cref="UseTls" />, <see cref="NamingMode" /> and <see cref="RegionCode" />
    ///     </para>
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>Controls the level of validation performed on bucket names. You should only create buckets that are valid DNS names.</summary>
    public BucketNameValidationMode BucketNameValidationMode { get; set; } = BucketNameValidationMode.Default;

    /// <summary>
    /// Controls the mode of validation that is applied to object keys. By default provider specific validation is performed. If you are not using a provider, it will default to
    /// <see cref="ObjectKeyValidationMode.Unrestricted" />.
    /// </summary>
    public ObjectKeyValidationMode ObjectKeyValidationMode { get; set; } = ObjectKeyValidationMode.Default;

    /// <summary>When EncodingType is Url, decode returned object keys, prefixes, delimiters, and StartAfter.</summary>
    /// <remarks>Key-based pagination markers are always decoded for reuse. Opaque continuation tokens, version IDs, and upload IDs are never URL decoded.</remarks>
    public bool AutoUrlDecodeResponses { get; set; }

    /// <summary>When enabled, SimpleS3 will always calculate the ContentMD5 property before sending the request</summary>
    public bool AlwaysCalculateContentMd5 { get; set; }

    /// <summary>Set this to true to make SimpleS3 throw exceptions when it receives an error response from the S3 API.</summary>
    public bool ThrowExceptionOnError { get; set; }

    /// <summary>Set this to true to include sensitive signing material and signed request query strings in logs.</summary>
    public bool LogSensitiveMaterial { get; set; }

    /// <summary>Provides the <c>RegionCode</c> API.</summary>
    public string RegionCode { get; set; } = string.Empty;

    /// <summary>Provides the <c>ProviderName</c> API.</summary>
    public string ProviderName { get; set; } = string.Empty;
}