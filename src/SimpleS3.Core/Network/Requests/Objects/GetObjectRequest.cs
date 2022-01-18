using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// This implementation of the GET operation retrieves objects from Amazon S3. To use GET, you must have READ access to the object. If you grant
/// READ access to the anonymous user, you can return the object without using an authorization header.
/// </summary>
public class GetObjectRequest : HeadObjectRequest, IHasRequestPayer
{
    internal GetObjectRequest() : base(HttpMethodType.GET) { }

    public GetObjectRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    public Payer RequestPayer { get; set; }

    public override void Reset()
    {
        RequestPayer = Payer.Unknown;

        base.Reset();
    }
}