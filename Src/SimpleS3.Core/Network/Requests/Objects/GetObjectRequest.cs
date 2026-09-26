using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// This implementation of the GET operation retrieves objects from Amazon S3. To use GET, you must have READ access to the object. If you grant READ access to the anonymous
/// user, you can return the object without using an authorization header.
/// </summary>
public class GetObjectRequest : HeadObjectRequest
{
    internal GetObjectRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetObjectRequest</c> API.</summary>
    public GetObjectRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }
}