using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Clients;

public interface ISignedObjectClient
{
    string SignPutObject(string bucketName, string objectKey, Stream? content, TimeSpan expires, Action<PutObjectRequest>? config = null);
    Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<SignedPutObjectRequest>? config = null, CancellationToken token = default);
    string SignGetObject(string bucketName, string objectKey, TimeSpan expires, Action<GetObjectRequest>? config = null);
    Task<GetObjectResponse> GetObjectAsync(string url, Action<SignedGetObjectRequest>? config = null, CancellationToken token = default);
    string SignDeleteObject(string bucketName, string objectKey, TimeSpan expires, Action<DeleteObjectRequest>? config = null);
    Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<SignedDeleteObjectRequest>? config = null, CancellationToken token = default);
    string SignHeadObject(string bucketName, string objectKey, TimeSpan expires, Action<HeadObjectRequest>? config = null);
    Task<HeadObjectResponse> HeadObjectAsync(string url, Action<SignedHeadObjectRequest>? config = null, CancellationToken token = default);
}