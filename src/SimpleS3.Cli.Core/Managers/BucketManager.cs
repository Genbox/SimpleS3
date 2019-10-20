using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.S3Types;
using Genbox.SimpleS3.Utils;

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

        public Task DeleteAsync(string bucketName)
        {
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            return RequestHelper.ExecuteRequestAsync(_client, c => c.DeleteBucketAsync(bucketName));
        }

        public IAsyncEnumerable<S3Object> GetAsync(string bucketName, bool includeOwner)
        {
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            return RequestHelper.ExecuteAsyncEnumerable(_client, c => c.ListObjectsRecursiveAsync(bucketName, includeOwner));
        }

        public async IAsyncEnumerable<S3Bucket> ListAsync(CancellationToken token)
        {
            ListBucketsResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.ListBucketsAsync(null, token)).ConfigureAwait(false);

            foreach (S3Bucket respBucket in resp.Buckets)
                yield return respBucket;
        }
    }
}