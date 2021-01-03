using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Aws;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3
{
    /// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
    public sealed class S3Client : IClient, IDisposable
    {
        private readonly ServiceProvider? _serviceProvider;
        private IBucketClient _bucketClient;
        private IMultipartClient _multipartClient;
        private IObjectClient _objectClient;

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, byte[] accessKey, AwsRegion region, IWebProxy? proxy = null) : this(new AwsConfig(new AccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(string keyId, string accessKey, AwsRegion region, IWebProxy? proxy = null) : this(new AwsConfig(new StringAccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="credentials">The credentials to use</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(IAccessKey credentials, AwsRegion region, IWebProxy? proxy = null) : this(new AwsConfig(credentials, region), proxy) { }

        /// <summary>Creates a new instance of <see cref="S3Client" /></summary>
        /// <param name="config">The configuration you want to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        public S3Client(AwsConfig config, IWebProxy? proxy = null) : this(Options.Create(config), proxy) { }

        public S3Client(IOptions<AwsConfig> options, IWebProxy? proxy = null)
        {
            ServiceCollection services = new ServiceCollection();
            IS3ClientBuilder builder = services.AddSimpleS3();

            if (proxy != null)
                builder.HttpBuilder.UseProxy(proxy);

            services.AddSingleton<IOptions<Config>>(options);

            _serviceProvider = services.BuildServiceProvider();
            IObjectClient objectClient = _serviceProvider.GetRequiredService<IObjectClient>();
            IBucketClient bucketClient = _serviceProvider.GetRequiredService<IBucketClient>();
            IMultipartClient multipartClient = _serviceProvider.GetRequiredService<IMultipartClient>();
            Transfer transfer = _serviceProvider.GetRequiredService<Transfer>();

            Initialize(objectClient, bucketClient, multipartClient, transfer);
        }

        public S3Client(IOptions<AwsConfig> options, INetworkDriver networkDriver, ILoggerFactory loggerFactory)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSimpleS3();
            services.Replace(ServiceDescriptor.Singleton(networkDriver));
            services.Replace(ServiceDescriptor.Singleton(loggerFactory));
            services.AddSingleton<IOptions<Config>>(options);

            _serviceProvider = services.BuildServiceProvider();
            IObjectClient objectClient = _serviceProvider.GetRequiredService<IObjectClient>();
            IBucketClient bucketClient = _serviceProvider.GetRequiredService<IBucketClient>();
            IMultipartClient multipartClient = _serviceProvider.GetRequiredService<IMultipartClient>();
            Transfer transfer = _serviceProvider.GetRequiredService<Transfer>();

            Initialize(objectClient, bucketClient, multipartClient, transfer);
        }

        public S3Client(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, Transfer transfer)
        {
            Initialize(objectClient, bucketClient, multipartClient, transfer);
        }

        public Transfer Transfer { get; private set; }

        public IObjectOperations ObjectOperations => _objectClient.ObjectOperations;
        public IBucketOperations BucketOperations => _bucketClient.BucketOperations;
        public IMultipartOperations MultipartOperations => _multipartClient.MultipartOperations;

        public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.ListObjectsAsync(bucketName, config, token);
        }

        public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.RestoreObjectAsync(bucketName, objectKey, config, token);
        }

        public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.CopyObjectAsync(sourceBucketName, sourceObjectKey, destinationBucket, destinationObjectKey, config, token);
        }

        public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.PutObjectAclAsync(bucketName, objectKey, config, token);
        }

        public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.GetObjectAclAsync(bucketName, objectKey, config, token);
        }

        public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.GetObjectLegalHoldAsync(bucketName, objectKey, config, token);
        }

        public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.PutObjectLegalHoldAsync(bucketName, objectKey, lockStatus, config, token);
        }

        public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.ListObjectVersionsAsync(bucketName, config, token);
        }

        public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.CreateBucketAsync(bucketName, config, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.DeleteBucketAsync(bucketName, config, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
        {
            return _multipartClient.ListMultipartUploadsAsync(bucketName, config, token);
        }

        public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.ListBucketsAsync(config, token);
        }

        public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.HeadBucketAsync(bucketName, config, token);
        }

        public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.PutBucketLockConfigurationAsync(bucketName, enabled, config, token);
        }

        public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.GetBucketLockConfigurationAsync(bucketName, config, token);
        }

        public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.GetBucketTaggingAsync(bucketName, config, token);
        }

        public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.PutBucketTaggingAsync(bucketName, tags, config, token);
        }

        public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.DeleteBucketTaggingAsync(bucketName, config, token);
        }

        public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.PutBucketAccelerateConfigurationAsync(bucketName, enabled, config, token);
        }

        public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.GetBucketAccelerateConfigurationAsync(bucketName, config, token);
        }

        public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.PutBucketLifecycleConfigurationAsync(bucketName, rules, config, token);
        }

        public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.PutBucketVersioningAsync(bucketName, enabled, config, token);
        }

        public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default)
        {
            return _bucketClient.GetBucketVersioningAsync(bucketName, config, token);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.DeleteObjectAsync(bucketName, objectKey, config, token);
        }

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.DeleteObjectsAsync(bucketName, objectKeys, config, token);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.HeadObjectAsync(bucketName, objectKey, config, token);
        }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            return _multipartClient.CreateMultipartUploadAsync(bucketName, objectKey, config, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default)
        {
            return _multipartClient.UploadPartAsync(bucketName, objectKey, partNumber, uploadId, content, config, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default)
        {
            return _multipartClient.ListPartsAsync(bucketName, objectKey, uploadId, config, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            return _multipartClient.CompleteMultipartUploadAsync(bucketName, objectKey, uploadId, parts, config, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            return _multipartClient.AbortMultipartUploadAsync(bucketName, objectKey, uploadId, config, token);
        }

        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.GetObjectAsync(bucketName, objectKey, config, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default)
        {
            return _objectClient.PutObjectAsync(bucketName, objectKey, data, config, token);
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }

        private void Initialize(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, Transfer transfer)
        {
            _objectClient = objectClient;
            _bucketClient = bucketClient;
            _multipartClient = multipartClient;
            Transfer = transfer;
        }
    }
}