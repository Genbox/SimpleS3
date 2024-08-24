using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts;

[PublicAPI]
public class SimpleS3Config
{
    public SimpleS3Config() {} //Needed for config binding

    public SimpleS3Config(string providerName, string endpoint)
    {
        ProviderName = providerName;
        Endpoint = endpoint;
    }

    public SimpleS3Config(IAccessKey credentials, string endpoint, string regionCode) : this(string.Empty, endpoint) //Used internally
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    /// <summary>The credentials to use when communicating with S3.</summary>
    public IAccessKey Credentials { get; set; }

    /// <summary>
    /// There are 3 different signing modes: 1. Unsigned - means the request will be sent without a signature at all. 2. FullSignature - Means the full payload will be hashed
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
    ///     Use this to set a custom endpoint. It can either be a URL like https://myserver.com:9101 or a template. When setting a endpoint to a URL <see cref="UseTls" />,
    ///     <see cref="NamingMode" /> and <see cref="RegionCode" /> will be ignored.
    ///     </para>
    ///     <para>
    ///     However, you can also set the endpoint to a template in the form of {Scheme}{Region:.}myserver.com:9101/{Bucket} and SimpleS3 will build an URL for you using
    ///     <see cref="UseTls" />, <see cref="NamingMode" /> and <see cref="RegionCode" />
    ///     </para>
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>Controls the level of validation performed on bucket names. You should only create buckets that are valid DNS names.</summary>
    public BucketNameValidationMode BucketNameValidationMode { get; set; } = BucketNameValidationMode.Default;

    /// <summary>
    /// Controls the mode of validation that is applied to object keys. By default provider specific validation is performed. If you are not using a provider, it will default to
    /// <see cref="ObjectKeyValidationMode.Unrestricted" />.
    /// </summary>
    public ObjectKeyValidationMode ObjectKeyValidationMode { get; set; } = ObjectKeyValidationMode.Default;

    /// <summary>If a response has EncodingType set to Url, SimpleS3 will automatically URL decode the encoded part of the response if this setting is true.</summary>
    public bool AutoUrlDecodeResponses { get; set; }

    /// <summary>When enabled, SimpleS3 will always calculate the ContentMD5 property before sending the request</summary>
    public bool AlwaysCalculateContentMd5 { get; set; }

    /// <summary>Set this to true to make SimpleS3 throw exceptions when it receives an error response from the S3 API.</summary>
    public bool ThrowExceptionOnError { get; set; }

    public string RegionCode { get; set; }

    public string ProviderName { get; set; }
}