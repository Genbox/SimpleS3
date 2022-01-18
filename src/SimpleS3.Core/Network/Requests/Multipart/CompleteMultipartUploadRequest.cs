using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart;

/// <summary>
/// This operation completes a multipart upload by assembling previously uploaded parts. You first initiate the multipart upload and then upload
/// all parts using the Upload Parts operation. After successfully uploading all relevant parts of an upload, you call this operation to complete the
/// upload. Upon receiving this request, Amazon S3 concatenates all the parts in ascending order by part number to create a new object. In the Complete
/// Multipart Upload request, you must provide the parts list. You must ensure the parts list is complete, this operation concatenates the parts you
/// provide in the list. For each part in the list, you must provide the part number and the ETag header value, returned after that part was uploaded.
/// Processing of a Complete Multipart Upload request could take several minutes to complete. After Amazon S3 begins processing the request, it sends an
/// HTTP response header that specifies a 200 OK response. While processing is in progress, Amazon S3 periodically sends whitespace characters to keep
/// the connection from timing out. Because a request could fail after the initial 200 OK response has been sent, it is important that you check the
/// response body to determine whether the request succeeded.
/// </summary>
public class CompleteMultipartUploadRequest : BaseRequest, IHasRequestPayer, IHasBucketName, IHasObjectKey, IHasUploadId
{
    internal CompleteMultipartUploadRequest() : base(HttpMethodType.POST)
    {
        UploadParts = new List<S3PartInfo>();
    }

    public CompleteMultipartUploadRequest(string bucketName, string objectKey, string uploadId, params UploadPartResponse[] parts) : this(bucketName, objectKey, uploadId, (IEnumerable<UploadPartResponse>)parts) { }

    public CompleteMultipartUploadRequest(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts) : this()
    {
        Initialize(bucketName, objectKey, uploadId, parts);
    }

    public IList<S3PartInfo> UploadParts { get; }
    public string BucketName { get; set; }
    public string ObjectKey { get; set; }
    public Payer RequestPayer { get; set; }
    public string UploadId { get; set; }

    internal void Initialize(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        UploadId = uploadId;

        foreach (UploadPartResponse part in parts)
        {
            UploadParts.Add(new S3PartInfo(part.ETag, part.PartNumber));
        }
    }

    public override void Reset()
    {
        RequestPayer = Payer.Unknown;
        UploadParts.Clear();

        base.Reset();
    }
}