using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>
/// Creates or modifies the PublicAccessBlock configuration for an Amazon S3 bucket. To use this operation, you must have the s3:PutBucketPublicAccessBlock permission. For more
/// information about Amazon S3 permissions, see Specifying Permissions in a Policy.
/// </summary>
public class PutPublicAccessBlockRequest : BaseRequest, IHasBucketName
{
    internal PutPublicAccessBlockRequest() : base(HttpMethodType.PUT) {}

    public PutPublicAccessBlockRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>
    ///     <para>
    ///     Specifies whether Amazon S3 should block public access control lists (ACLs) for this bucket and objects in this bucket. Setting this element to TRUE causes the following
    ///     behavior:
    ///     </para>
    ///     <list type="bullet">
    ///         <item>PUT Bucket ACL and PUT Object ACL calls fail if the specified ACL is public.</item> <item>PUT Object calls fail if the request includes a public ACL.</item>
    ///         <item>PUT Bucket calls fail if the request includes a public ACL.</item>
    ///     </list>
    ///     <para>Enabling this setting doesn't affect existing policies or ACLs.</para>
    /// </summary>
    public bool BlockPublicAcls { get; set; }

    /// <summary>
    ///     <para>
    ///     Specifies whether Amazon S3 should block public bucket policies for this bucket. Setting this element to TRUE causes Amazon S3 to reject calls to PUT Bucket policy if
    ///     the specified bucket policy allows public access.
    ///     </para>
    ///     <para>Enabling this setting doesn't affect existing bucket policies.</para>
    /// </summary>
    public bool BlockPublicPolicy { get; set; }

    /// <summary>
    ///     <para>
    ///     Specifies whether Amazon S3 should ignore public ACLs for this bucket and objects in this bucket. Setting this element to TRUE causes Amazon S3 to ignore all public ACLs
    ///     on this bucket and objects in this bucket.
    ///     </para>
    ///     <para>Enabling this setting doesn't affect the persistence of any existing ACLs and doesn't prevent new public ACLs from being set.</para>
    /// </summary>
    public bool IgnorePublicAcls { get; set; }

    /// <summary>
    ///     <para>
    ///     Specifies whether Amazon S3 should restrict public bucket policies for this bucket. Setting this element to TRUE restricts access to this bucket to only AWS service
    ///     principals and authorized users within this account if the bucket has a public policy.
    ///     </para>
    ///     <para>
    ///     Enabling this setting doesn't affect previously stored bucket policies, except that public and cross-account access within any public bucket policy, including non-public
    ///     delegation to specific accounts, is blocked.
    ///     </para>
    /// </summary>
    public bool RestrictPublicBuckets { get; set; }

    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }

    public override void Reset()
    {
        BlockPublicAcls = false;
        BlockPublicPolicy = false;
        IgnorePublicAcls = false;
        RestrictPublicBuckets = false;

        base.Reset();
    }
}