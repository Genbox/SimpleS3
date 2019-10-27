using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Requests.Objects
{
    /// <summary>
    /// This operation aborts a multipart upload. After a multipart upload is aborted, no additional parts can be uploaded using that upload ID. The
    /// storage consumed by any previously uploaded parts will be freed. However, if any part uploads are currently in progress, those part uploads might or
    /// might not succeed. As a result, it might be necessary to abort a given multipart upload multiple times in order to completely free all storage
    /// consumed by all parts. To verify that all parts have been removed, so you don't get charged for the part storage, you should call the List Parts
    /// operation and ensure the parts list is empty.
    /// </summary>
    public class AbortMultipartUploadRequest : BaseRequest
    {
        public AbortMultipartUploadRequest(string bucketName, string objectKey, string uploadId) : base(HttpMethod.DELETE, bucketName, objectKey)
        {
            UploadId = uploadId;
        }

        public string UploadId { get; }
    }
}