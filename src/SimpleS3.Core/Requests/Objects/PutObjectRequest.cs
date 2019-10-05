using System.IO;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Requests.Objects
{
    /// <summary>
    /// This implementation of the PUT operation adds an object to a bucket. You must have WRITE permissions on a bucket to add an object to it.
    /// Amazon S3 never adds partial objects; if you receive a success response, Amazon S3 added the entire object to the bucket.
    /// </summary>
    public class PutObjectRequest : InitiateMultipartUploadRequest
    {
        public PutObjectRequest(string bucketName, string resource, Stream data) : base(bucketName, resource)
        {
            Method = HttpMethod.PUT;
            Content = data;
        }
    }
}