using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Core.Responses.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3BucketClient : IS3BucketClient
    {
        private readonly IBucketOperations _bucketOperations;
        private readonly IS3ObjectClient _objectClient;

        public S3BucketClient(IBucketOperations bucketOperations, IS3ObjectClient objectClient)
        {
            _bucketOperations = bucketOperations;
            _objectClient = objectClient;
        }

        public Task<GetBucketResponse> GetBucketAsync(string bucketName, Action<GetBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketOperations.GetAsync(bucketName, config, token);
        }

        public Task<PutBucketResponse> PutBucketAsync(string bucketName, Action<PutBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketOperations.PutAsync(bucketName, config, token);
        }

        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest> config = null, CancellationToken token = default)
        {
            return _bucketOperations.DeleteAsync(bucketName, config, token);
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest> config = null, CancellationToken token = default)
        {
            return _bucketOperations.ListMultipartUploadsAsync(bucketName, config, token);
        }

        public async Task<DeleteBucketStatus> EmptyBucket(string bucketName, CancellationToken token = default)
        {
            //TODO: this can be optimized if we don't use GetBucketRecursiveAsync, but instead call the methods directly
            List<S3DeleteInfo> tempList = new List<S3DeleteInfo>(1000);

            await foreach (S3Object obj in this.GetBucketRecursiveAsync(bucketName, token: token))
            {
                tempList.Add(new S3DeleteInfo(obj.Name, null));

                if (tempList.Count != 1000)
                    continue;

                DeleteMultipleObjectsResponse multiDelResponse = await _objectClient.DeleteMultipleObjectsAsync(bucketName, tempList, request => request.Quiet = true, token).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    return DeleteBucketStatus.FailedToDeleteObject;

                tempList.Clear();
            }

            if (tempList.Count > 0)
            {
                DeleteMultipleObjectsResponse multiDelResponse = await _objectClient.DeleteMultipleObjectsAsync(bucketName, tempList, request => request.Quiet = true, token).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    return DeleteBucketStatus.FailedToDeleteObject;
            }

            return DeleteBucketStatus.Ok;
        }
    }
}