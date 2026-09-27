using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Clients;

/// <summary>Provides the <c>IObjectClient</c> API.</summary>
[PublicAPI]
public interface IObjectClient
{
    /// <summary>Delete an object</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default);

    /// <summary>Delete multiple objects</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKeys">A list of object keys</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default);

    /// <summary>Head an object. Can be used to check if an object exists without downloading the content of it.</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default);

    /// <summary>Get (download) an object</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default);

    /// <summary>Put (upload) an object</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="data">The content of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default);

    /// <summary>List all objects within a bucket</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default);

    /// <summary>Copies an object already present in an S3 bucket to a new object in the same or another S3 bucket</summary>
    /// <param name="sourceBucketName">The source bucket</param>
    /// <param name="sourceObjectKey">The source object key</param>
    /// <param name="destinationBucket">The destination bucket</param>
    /// <param name="destinationObjectKey">The destination object key</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default);

    /// <summary>Set the access control list (ACL) permissions for an object that already exists in a bucket. You must have WRITE_ACP permission to set the ACL of an object.</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default);

    /// <summary>Returns the access control list (ACL) of an object. To use this operation, you must have READ_ACP access to the object.</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetObjectRetentionAsync</c> API.</summary>
    Task<GetObjectRetentionResponse> GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutObjectRetentionAsync</c> API.</summary>
    Task<PutObjectRetentionResponse> PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>ListObjectVersionsAsync</c> API.</summary>
    Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default);

    /// <summary>Updates an object's SSE-KMS encryption configuration.</summary>
    Task<UpdateObjectEncryptionResponse> UpdateObjectEncryptionAsync(string bucketName, string objectKey, string kmsKeyArn, Action<UpdateObjectEncryptionRequest>? config = null, CancellationToken token = default);

    /// <summary>Restores an object from Amazon Glacier</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetObjectAttributesAsync</c> API.</summary>
    Task<GetObjectAttributesResponse> GetObjectAttributesAsync(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes, Action<GetObjectAttributesRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetObjectAnnotationAsync</c> API.</summary>
    Task<GetObjectAnnotationResponse> GetObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<GetObjectAnnotationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutObjectAnnotationAsync</c> API.</summary>
    Task<PutObjectAnnotationResponse> PutObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Stream content, Action<PutObjectAnnotationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteObjectAnnotationAsync</c> API.</summary>
    Task<DeleteObjectAnnotationResponse> DeleteObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<DeleteObjectAnnotationRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>ListObjectAnnotationsAsync</c> API.</summary>
    Task<ListObjectAnnotationsResponse> ListObjectAnnotationsAsync(string bucketName, string objectKey, Action<ListObjectAnnotationsRequest>? config = null, CancellationToken token = default);

    /// <summary>Gets an object's current Legal Hold status.</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default);

    /// <summary>Applies a Legal Hold configuration to the specified object.</summary>
    /// <param name="bucketName">Name of the bucket</param>
    /// <param name="objectKey">The key of the object</param>
    /// <param name="lockStatus">Set to true if you want to lock the object, otherwise, set to false to disable a lock</param>
    /// <param name="config">A delegate to configure the request</param>
    /// <param name="token">A cancellation token</param>
    Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>GetObjectTaggingAsync</c> API.</summary>
    Task<GetObjectTaggingResponse> GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>PutObjectTaggingAsync</c> API.</summary>
    Task<PutObjectTaggingResponse> PutObjectTaggingAsync(string bucketName, string objectKey, IDictionary<string, string> tags, Action<PutObjectTaggingRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>DeleteObjectTaggingAsync</c> API.</summary>
    Task<DeleteObjectTaggingResponse> DeleteObjectTaggingAsync(string bucketName, string objectKey, Action<DeleteObjectTaggingRequest>? config = null, CancellationToken token = default);
}