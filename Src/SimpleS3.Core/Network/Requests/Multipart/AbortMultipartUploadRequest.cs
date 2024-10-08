﻿using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart;

/// <summary>
/// This operation aborts a multipart upload. After a multipart upload is aborted, no additional parts can be uploaded using that upload ID. The storage consumed by any
/// previously uploaded parts will be freed. However, if any part uploads are currently in progress, those part uploads might or might not succeed. As a result, it might be necessary
/// to abort a given multipart upload multiple times in order to completely free all storage consumed by all parts. To verify that all parts have been removed, so you don't get
/// charged for the part storage, you should call the List Parts operation and ensure the parts list is empty.
/// </summary>
public class AbortMultipartUploadRequest : BaseRequest, IHasRequestPayer, IHasBucketName, IHasObjectKey, IHasUploadId
{
    internal AbortMultipartUploadRequest() : base(HttpMethodType.DELETE) {}

    public AbortMultipartUploadRequest(string bucketName, string objectKey, string uploadId) : this()
    {
        Initialize(bucketName, objectKey, uploadId);
    }

    public string BucketName { get; set; } = null!;
    public string ObjectKey { get; set; } = null!;
    public Payer RequestPayer { get; set; }
    public string UploadId { get; set; } = null!;

    internal void Initialize(string bucketName, string objectKey, string uploadId)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        UploadId = uploadId;
    }

    public override void Reset()
    {
        RequestPayer = Payer.Unknown;

        base.Reset();
    }
}