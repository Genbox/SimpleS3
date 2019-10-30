using System.IO;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// This implementation of the PUT operation adds an object to a bucket. You must have WRITE permissions on a bucket to add an object to it.
    /// Amazon S3 never adds partial objects; if you receive a success response, Amazon S3 added the entire object to the bucket.
    /// </summary>
    public class PutObjectRequest : CreateMultipartUploadRequest
    {
        public PutObjectRequest(string bucketName, string objectKey, Stream data) : base(bucketName, objectKey)
        {
            Method = HttpMethod.PUT;
            Content = data;
        }
    }
}