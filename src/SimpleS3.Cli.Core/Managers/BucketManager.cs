using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Cli.Core.Managers
{
    public class BucketManager
    {
        private readonly IS3Client _client;

        public BucketManager(IS3Client client)
        {
            _client = client;
        }

        public Task CreateAsync(string bucketName)
        {
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            return RequestHelper.ExecuteRequestAsync(_client, c => c.CreateBucketAsync(bucketName));
        }

        public async Task EmptyAsync(string bucketName)
        {
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            if (await _client.DeleteAllObjectsAsync(bucketName).ConfigureAwait(false) != DeleteAllObjectsStatus.Ok)
                throw new Exception("Failed to empty bucket");
        }

        public async Task DeleteAsync(string bucketName)
        {
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            DeleteAllObjectsStatus delResp = await _client.DeleteAllObjectsAsync(bucketName).ConfigureAwait(false);

            if (delResp != DeleteAllObjectsStatus.Ok)
                throw new Exception("Failed to delete bucket");

            DeleteBucketResponse delResp2 = await _client.DeleteBucketAsync(bucketName).ConfigureAwait(false);

            if (!delResp2.IsSuccess)
                throw new Exception("Failed to delete bucket");
        }

        public async IAsyncEnumerable<S3Bucket> ListAsync(CancellationToken token)
        {
            ListBucketsResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.ListBucketsAsync(null, token)).ConfigureAwait(false);

            foreach (S3Bucket respBucket in resp.Buckets)
                yield return respBucket;
        }
    }
}