using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Cli.Core.Helpers;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Cli.Core.Managers
{
    public class BucketManager
    {
        private readonly IClient _client;

        public BucketManager(IClient client)
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

            await _client.DeleteAllObjectsAsync(bucketName).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string bucketName)
        {
            Validator.RequireNotNullOrEmpty(bucketName, nameof(bucketName));

            await _client.DeleteAllObjectsAsync(bucketName).ConfigureAwait(false);
            await _client.DeleteBucketAsync(bucketName).ConfigureAwait(false);
        }

        public async IAsyncEnumerable<S3Bucket> ListAsync([EnumeratorCancellation] CancellationToken token)
        {
            ListBucketsResponse resp = await RequestHelper.ExecuteRequestAsync(_client, c => c.ListBucketsAsync(null, token)).ConfigureAwait(false);

            foreach (S3Bucket respBucket in resp.Buckets)
                yield return respBucket;
        }
    }
}