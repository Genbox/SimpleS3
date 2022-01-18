using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations;

[PublicAPI]
public interface IObjectOperations
{
    /// <summary>Deletes an object. See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_DeleteObject.html for details</summary>
    Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken token = default);

    /// <summary>Check if an object exists. See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_HeadObject.html for details</summary>
    Task<HeadObjectResponse> HeadObjectAsync(HeadObjectRequest request, CancellationToken token = default);

    /// <summary>Delete multiple objects in one request. See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_DeleteObjects.html for details</summary>
    Task<DeleteObjectsResponse> DeleteObjectsAsync(DeleteObjectsRequest request, CancellationToken token = default);

    /// <summary>Put an object into the S3 bucket. See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_PutObject.html for details</summary>
    Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken token = default);

    /// <summary>Get an object from an S3 bucket. See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_GetObject.html for details</summary>
    Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request, CancellationToken token = default);

    /// <summary>List objects within a bucket. See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListObjectsV2.html for details</summary>
    Task<ListObjectsResponse> ListObjectsAsync(ListObjectsRequest request, CancellationToken token = default);

    /// <summary>List all versions of objects within a bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_ListObjectVersions.html for details</summary>
    Task<ListObjectVersionsResponse> ListObjectVersionsAsync(ListObjectVersionsRequest request, CancellationToken token = default);

    /// <summary>
    /// Restores an archived copy of an object back into Amazon S3. See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_RestoreObject.html
    /// for details
    /// </summary>
    Task<RestoreObjectResponse> RestoreObjectsAsync(RestoreObjectRequest request, CancellationToken token = default);

    /// <summary>
    /// Copies an object that is already present in an S3 bucket. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_CopyObject.html for
    /// details
    /// </summary>
    Task<CopyObjectResponse> CopyObjectsAsync(CopyObjectRequest request, CancellationToken token = default);

    /// <summary>
    /// Set the access control list (ACL) permissions for an object that already exists in a bucket. See
    /// https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutObjectAcl.html for details
    /// </summary>
    Task<PutObjectAclResponse> PutObjectAclAsync(PutObjectAclRequest request, CancellationToken token = default);

    /// <summary>Returns the access control list (ACL) of an object. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetObjectAcl.html for details</summary>
    Task<GetObjectAclResponse> GetObjectAclAsync(GetObjectAclRequest request, CancellationToken token = default);

    /// <summary>Gets an object's current Legal Hold status. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetObjectLegalHold.html for details</summary>
    Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(GetObjectLegalHoldRequest request, CancellationToken token = default);

    /// <summary>
    /// Applies a Legal Hold configuration to the specified object. See https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutObjectLegalHold.html
    /// for details
    /// </summary>
    Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(PutObjectLegalHoldRequest request, CancellationToken token = default);
}