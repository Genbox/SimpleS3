using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Fluid;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3
{
    /// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
    public sealed class S3Client : IDisposable, IS3Client
    {
        private readonly IS3BucketClient _bucketClient;
        private readonly IS3ObjectClient _objectClient;
        private readonly ServiceProvider _provider;

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, byte[] accessKey, AwsRegion region, WebProxy proxy = null) : this(new S3Config(new AccessKey(keyId, accessKey), region), proxy)
        {
        }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, string accessKey, AwsRegion region, WebProxy proxy = null) : this(new S3Config(new StringAccessKey(keyId, accessKey), region), proxy)
        {
        }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="config">The configuration you want to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(S3Config config, IWebProxy proxy = null) : this(config, new HttpClientHandler { Proxy = proxy })
        {
        }

        public S3Client(S3Config config, HttpMessageHandler messageHandler)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton(x => Options.Create(config));
            services.AddSimpleS3(null, messageHandler);
            _provider = services.BuildServiceProvider();
            _objectClient = _provider.GetRequiredService<IS3ObjectClient>();
            _bucketClient = _provider.GetRequiredService<IS3BucketClient>();

            Transfer = new Transfer(_objectClient.ObjectOperations);
        }

        public S3Client(IS3ObjectClient objectClient, IS3BucketClient bucketClient)
        {
            _objectClient = objectClient;
            _bucketClient = bucketClient;

            Transfer = new Transfer(_objectClient.ObjectOperations);
        }

        public Transfer Transfer { get; }

        public void Dispose()
        {
            _provider?.Dispose();
        }

        public IObjectOperations ObjectOperations => _objectClient.ObjectOperations;
        public IBucketOperations BucketOperations => _bucketClient.BucketOperations;

        public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.ListObjectsAsync(bucketName, config, token);
        }

        public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.CreateBucketAsync(bucketName, config, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.DeleteBucketAsync(bucketName, config, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.ListMultipartUploadsAsync(bucketName, config, token);
        }

        public Task<EmptyBucketStatus> EmptyBucketAsync(string bucketName, CancellationToken token = default)
        {
            return _bucketClient.EmptyBucketAsync(bucketName, token);
        }

        public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest> config = null, CancellationToken token = default)
        {
            return _bucketClient.ListBucketsAsync(config, token);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.DeleteObjectAsync(bucketName, objectKey, config, token);
        }

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.DeleteObjectsAsync(bucketName, objectKeys, config, token);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.HeadObjectAsync(bucketName, objectKey, config, token);
        }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.CreateMultipartUploadAsync(bucketName, objectKey, config, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.ListPartsAsync(bucketName, objectKey, uploadId, config, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);
        }

        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.GetObjectAsync(bucketName, objectKey, config, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.PutObjectAsync(bucketName, objectKey, data, config, token);
        }

        public Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest> config = null, CancellationToken token = default)
        {
            return _objectClient.MultipartUploadAsync(bucketName, objectKey, data, partSize, numParallelParts, config, token);
        }

        public Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int numParallelParts = 4, int bufferSize = 16777216, CancellationToken token = default)
        {
            return _objectClient.MultipartDownloadAsync(bucketName, objectKey, output, numParallelParts, bufferSize, token);
        }
    }
}