using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Requests.Objects
{
    /// <summary>
    /// The HEAD operation retrieves metadata from an object without returning the object itself. This operation is useful if you are interested
    /// only in an object's metadata. To use HEAD, you must have READ access to the object. A HEAD request has the same options as a GET operation on an
    /// object. The response is identical to the GET response except that there is no response body.
    /// </summary>
    public class HeadObjectRequest : GetObjectRequest
    {
        public HeadObjectRequest(string bucketName, string objectKey) : base(bucketName, objectKey)
        {
            Method = HttpMethod.HEAD;
        }
    }
}