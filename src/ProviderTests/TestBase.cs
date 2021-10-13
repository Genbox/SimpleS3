using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests
{
    public abstract class TestBase
    {
        protected string GetTestBucket(IProfile profile)
        {
            return UtilityHelper.GetTestBucket(profile);
        }

        protected string GetTemporaryBucket()
        {
            return UtilityHelper.GetTemporaryBucket();
        }

        protected async Task CreateTempBucketAsync(ISimpleClient client, Func<string, Task> action, Action<CreateBucketRequest>? config = null)
        {
            string tempBucketName = GetTemporaryBucket();

            CreateBucketResponse createResponse = await client.CreateBucketAsync(tempBucketName, config).ConfigureAwait(false);
            Assert.True(createResponse.IsSuccess);

            try
            {
                await action(tempBucketName).ConfigureAwait(false);
            }
            finally
            {
                int errors = await DeleteAllObjects(client, tempBucketName);

                Assert.Equal(0, errors);

                DeleteBucketResponse del2Resp = await client.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
                Assert.True(del2Resp.IsSuccess);
            }
        }

        private static async Task<int> DeleteAllObjects(IObjectClient client, string bucket)
        {
            ListObjectVersionsResponse response;
            Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucket);

            int failed = 0;

            do
            {
                response = await responseTask;

                if (!response.IsSuccess)
                    return -1;

                if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                    break;

                if (response.IsTruncated)
                {
                    string keyMarker = response.NextKeyMarker;
                    responseTask = client.ListObjectVersionsAsync(bucket, req => req.KeyMarker = keyMarker);
                }

                IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                           .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

                DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucket, delete, req => req.Quiet = true).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    return -1;

                failed += multiDelResponse.Errors.Count;

            } while (response.IsTruncated);

            return failed;
        }
    }
}