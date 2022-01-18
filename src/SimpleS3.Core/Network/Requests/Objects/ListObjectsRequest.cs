using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// List the content of a bucket in chunks of up to 1000 objects at the time. To use this operation in an AWS Identity and Access Management
/// (IAM) policy, you must have permissions to perform the s3:ListBucket action. The bucket owner has this permission by default and can grant this
/// permission to others.
/// </summary>
public class ListObjectsRequest : BaseRequest, IHasRequestPayer, IHasBucketName
{
    internal ListObjectsRequest() : base(HttpMethodType.GET) { }

    public ListObjectsRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>
    /// A delimiter is a character you use to group keys. If you specify a <see cref="Prefix" />, all of the keys that contain the same string
    /// between the prefix and the first occurrence of the delimiter after the prefix are grouped under a single result element called CommonPrefixes. If you
    /// don't specify the prefix parameter, the substring starts at the beginning of the key. The keys that are grouped under the CommonPrefixes result
    /// element are not returned elsewhere in the response.
    /// </summary>
    public string? Delimiter { get; set; }

    /// <summary>
    /// An object key can contain any Unicode character. However, XML 1.0 parsers cannot parse some characters, such as characters with an ASCII
    /// value from 0 to 10. For characters that are not supported in XML 1.0, you can add this parameter to request that Amazon S3 encode the keys in the
    /// response.
    /// </summary>
    public EncodingType EncodingType { get; set; }

    /// <summary>
    /// Sets the maximum number of keys returned in the response body. If you want to retrieve fewer than the default 1,000 keys, you can add this
    /// to your request. The response might contain fewer keys, but it never contains more. If there are additional keys that satisfy the search criteria,
    /// but these keys were not returned because max-keys was exceeded, the response contains <IsTruncated>true</IsTruncated>. To return the additional keys,
    /// see NextContinuationToken in the response.
    /// </summary>
    public int? MaxKeys { get; set; }

    /// <summary>
    /// Limits the response to keys that begin with the specified prefix. You can use prefixes to separate a bucket into different groupings of
    /// keys. You can think of using prefix to make groups in the same way you'd use a folder in a file system.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// When the response to this API call is truncated - that is, the IsTruncated response element value is true, the response also includes the
    /// NextContinuationToken element. To list the next set of objects, you can use the NextContinuationToken element in the next request as the
    /// continuation-token.
    /// </summary>
    public string? ContinuationToken { get; set; }

    /// <summary>
    /// By default, the API does not return the Owner information in the response. If you want the owner information in the response, you can
    /// specify this parameter with the value set to true.
    /// </summary>
    public bool? FetchOwner { get; set; }

    /// <summary>
    /// If you want the API to return key names after a specific object key in your key space, you can add this parameter. Amazon S3 lists objects
    /// in UTF-8 character encoding in lexicographical order. This parameter is valid only in your first request. If the response is truncated, you can
    /// specify this parameter along with the continuation-token parameter, and then Amazon S3 ignores this parameter.
    /// </summary>
    public string? StartAfter { get; set; }

    public string BucketName { get; set; }

    public Payer RequestPayer { get; set; }

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }

    public override void Reset()
    {
        Delimiter = null;
        MaxKeys = null;
        EncodingType = EncodingType.Unknown;
        Prefix = null;
        ContinuationToken = null;
        FetchOwner = null;
        StartAfter = null;
        RequestPayer = Payer.Unknown;

        base.Reset();
    }
}