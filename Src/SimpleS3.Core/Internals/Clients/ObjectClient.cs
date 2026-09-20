using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Clients;

internal sealed class ObjectClient(IObjectOperations operations) : IObjectClient
{
    public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteObjectRequest>.Shared.RentAndUseAsync(Setup, ActionAsync);

        void Setup(DeleteObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }

        Task<DeleteObjectResponse> ActionAsync(DeleteObjectRequest request) => operations.DeleteObjectAsync(request, token);
    }

    public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteObjectsRequest>.Shared.RentAndUseAsync(Setup, request => operations.DeleteObjectsAsync(request, token));

        void Setup(DeleteObjectsRequest req)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            req.Initialize(bucketName, objectKeys);
            config?.Invoke(req);
        }
    }

    public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<HeadObjectRequest>.Shared.RentAndUseAsync(Setup, request => operations.HeadObjectAsync(request, token));

        void Setup(HeadObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetObjectAsync(request, token));

        void Setup(GetObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectRequest>.Shared.RentAndUseAsync(Setup, request => operations.PutObjectAsync(request, token));

        void Setup(PutObjectRequest req)
        {
            req.Initialize(bucketName, objectKey, data);
            config?.Invoke(req);
        }
    }

    public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListObjectsRequest>.Shared.RentAndUseAsync(Setup, request => operations.ListObjectsAsync(request, token));

        void Setup(ListObjectsRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<RestoreObjectRequest>.Shared.RentAndUseAsync(Setup, request => operations.RestoreObjectsAsync(request, token));

        void Setup(RestoreObjectRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<CopyObjectRequest>.Shared.RentAndUseAsync(Setup, request => operations.CopyObjectsAsync(request, token));

        void Setup(CopyObjectRequest req)
        {
            req.Initialize(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectAclRequest>.Shared.RentAndUseAsync(Setup, request => operations.PutObjectAclAsync(request, token));

        void Setup(PutObjectAclRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectAclRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetObjectAclAsync(request, token));

        void Setup(GetObjectAclRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectAttributesResponse> GetObjectAttributesAsync(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes, Action<GetObjectAttributesRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectAttributesRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetObjectAttributesAsync(request, token));

        void Setup(GetObjectAttributesRequest req)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            req.Initialize(bucketName, objectKey, objectAttributes);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectAnnotationResponse> GetObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<GetObjectAnnotationRequest>? config = null, CancellationToken token = default) => ObjectPool<GetObjectAnnotationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, objectKey, annotationName);
        config?.Invoke(req);
    }, req => operations.GetObjectAnnotationAsync(req, token));

    public Task<PutObjectAnnotationResponse> PutObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Stream content, Action<PutObjectAnnotationRequest>? config = null, CancellationToken token = default) => ObjectPool<PutObjectAnnotationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, objectKey, annotationName, content);
        config?.Invoke(req);
    }, req => operations.PutObjectAnnotationAsync(req, token));

    public Task<DeleteObjectAnnotationResponse> DeleteObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<DeleteObjectAnnotationRequest>? config = null, CancellationToken token = default) => ObjectPool<DeleteObjectAnnotationRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, objectKey, annotationName);
        config?.Invoke(req);
    }, req => operations.DeleteObjectAnnotationAsync(req, token));

    public Task<ListObjectAnnotationsResponse> ListObjectAnnotationsAsync(string bucketName, string objectKey, Action<ListObjectAnnotationsRequest>? config = null, CancellationToken token = default) => ObjectPool<ListObjectAnnotationsRequest>.Shared.RentAndUseAsync(req =>
    {
        req.Initialize(bucketName, objectKey);
        config?.Invoke(req);
    }, req => operations.ListObjectAnnotationsAsync(req, token));

    public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectLegalHoldRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetObjectLegalHoldAsync(request, token));

        void Setup(GetObjectLegalHoldRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectLegalHoldRequest>.Shared.RentAndUseAsync(Setup, request => operations.PutObjectLegalHoldAsync(request, token));

        void Setup(PutObjectLegalHoldRequest req)
        {
            req.Initialize(bucketName, objectKey, lockStatus);
            config?.Invoke(req);
        }
    }

    public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<ListObjectVersionsRequest>.Shared.RentAndUseAsync(Setup, request => operations.ListObjectVersionsAsync(request, token));

        void Setup(ListObjectVersionsRequest req)
        {
            req.Initialize(bucketName);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectTaggingResponse> GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectTaggingRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetObjectTaggingAsync(request, token));

        void Setup(GetObjectTaggingRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectTaggingResponse> PutObjectTaggingAsync(string bucketName, string objectKey, IDictionary<string, string> tags, Action<PutObjectTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectTaggingRequest>.Shared.RentAndUseAsync(Setup, request => operations.PutObjectTaggingAsync(request, token));

        void Setup(PutObjectTaggingRequest req)
        {
            req.Initialize(bucketName, objectKey, tags);
            config?.Invoke(req);
        }
    }

    public Task<DeleteObjectTaggingResponse> DeleteObjectTaggingAsync(string bucketName, string objectKey, Action<DeleteObjectTaggingRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<DeleteObjectTaggingRequest>.Shared.RentAndUseAsync(Setup, request => operations.DeleteObjectTaggingAsync(request, token));

        void Setup(DeleteObjectTaggingRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<GetObjectRetentionResponse> GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<GetObjectRetentionRequest>.Shared.RentAndUseAsync(Setup, request => operations.GetObjectRetentionAsync(request, token));

        void Setup(GetObjectRetentionRequest req)
        {
            req.Initialize(bucketName, objectKey);
            config?.Invoke(req);
        }
    }

    public Task<PutObjectRetentionResponse> PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config = null, CancellationToken token = default)
    {
        return ObjectPool<PutObjectRetentionRequest>.Shared.RentAndUseAsync(Setup, request => operations.PutObjectRetentionAsync(request, token));

        void Setup(PutObjectRetentionRequest req)
        {
            req.Initialize(bucketName, objectKey, lockMode, retainUntil);
            config?.Invoke(req);
        }
    }
}