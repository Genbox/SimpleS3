using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.TestBase
{
    public abstract class OnlineTestBase : UnitTestBase
    {

        protected OnlineTestBase(ITestOutputHelper outputHelper, string profileName) : base(outputHelper, profileName)
        {
            IProfileManager profileManager = Services.GetRequiredService<IProfileManager>();

            IProfile? profile = profileManager.GetProfile(profileName);

            if (profile == null)
                throw new InvalidOperationException($"Profile '{profileName}' not found. Remember to run the TestSetup utility");

            string uniqId = profile.KeyId.Substring(0, 8);
            BucketName = "testbucket-" + uniqId.ToLowerInvariant();
        }

        protected override void ConfigureCoreBuilder(ICoreBuilder coreBuilder, IConfigurationRoot configuration)
        {
            IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
            httpBuilder.UseTimeoutPolicy(TimeSpan.FromMinutes(10));

            IConfigurationSection proxySection = configuration.GetSection("Proxy");

            if (proxySection != null && proxySection["UseProxy"].Equals("true", StringComparison.OrdinalIgnoreCase))
                httpBuilder.UseProxy(proxySection["ProxyAddress"]);

            if (ProfileName != null)
            {
                coreBuilder.UseProfileManager()
                           .BindConfigToProfile(ProfileName);
            }

            base.ConfigureCoreBuilder(coreBuilder, configuration);
        }

        protected async Task<PutObjectResponse> UploadAsync(string bucketName, string objectKey, string content, Action<PutObjectRequest>? config = null, bool assumeSuccess = true)
        {
            await using MemoryStream ms = new MemoryStream(Constants.Utf8NoBom.GetBytes(content));
            PutObjectResponse resp = await ObjectClient.PutObjectAsync(bucketName, objectKey, ms, config).ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadAsync(string bucketName, string objectKey, Action<PutObjectRequest>? config = null, bool assumeSuccess = true)
        {
            return UploadAsync(bucketName, objectKey, "test", config, assumeSuccess);
        }

        protected Task<PutObjectResponse> UploadAsync(string objectKey, Action<PutObjectRequest>? config = null, bool assumeSuccess = true)
        {
            return UploadAsync(BucketName, objectKey, config, assumeSuccess);
        }

        protected async Task<PutObjectResponse> UploadTransferAsync(string bucketName, string objectKey, Action<IUpload>? action = null, bool assumeSuccess = true)
        {
            IUpload upload = Transfer.CreateUpload(bucketName, objectKey);
            action?.Invoke(upload);

            PutObjectResponse resp = await upload.UploadStringAsync("test", Encoding.UTF8).ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadTransferAsync(string objectKey, Action<IUpload>? action = null, bool assumeSuccess = true)
        {
            return UploadTransferAsync(BucketName, objectKey, action, assumeSuccess);
        }

        protected async Task<GetObjectResponse> AssertAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, bool assumeSuccess = true)
        {
            GetObjectResponse resp = await ObjectClient.GetObjectAsync(bucketName, objectKey, config).ConfigureAwait(false);

            if (assumeSuccess)
            {
                Assert.True(resp.IsSuccess);
                Assert.Equal("test", await resp.Content!.AsStringAsync().ConfigureAwait(false));
            }

            return resp;
        }

        protected Task<GetObjectResponse> AssertAsync(string objectKey, Action<GetObjectRequest>? config = null, bool assumeSuccess = true)
        {
            return AssertAsync(BucketName, objectKey, config, assumeSuccess);
        }

        protected async Task<GetObjectResponse> AssertTransferAsync(string bucketName, string objectKey, Action<IDownload>? config = null, bool assumeSuccess = true)
        {
            IDownload download = Transfer.CreateDownload(bucketName, objectKey);
            config?.Invoke(download);
            GetObjectResponse resp = await download.DownloadAsync().ConfigureAwait(false);

            if (assumeSuccess)
            {
                Assert.True(resp.IsSuccess);
                Assert.Equal("test", await resp.Content!.AsStringAsync().ConfigureAwait(false));
            }

            return resp;
        }

        protected Task<GetObjectResponse> AssertTransferAsync(string objectKey, Action<IDownload>? config = null, bool assumeSuccess = true)
        {
            return AssertTransferAsync(BucketName, objectKey, config, assumeSuccess);
        }

        protected async Task CreateTempBucketAsync(Func<string, Task> action)
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse createResponse = await BucketClient.CreateBucketAsync(tempBucketName).ConfigureAwait(false);
            Assert.True(createResponse.IsSuccess);

            try
            {
                await action(tempBucketName).ConfigureAwait(false);
            }
            finally
            {
                int errors = await DeleteAllObjects(ObjectClient, tempBucketName);

                Assert.Equal(0, errors);

                DeleteBucketResponse del2Resp = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
                Assert.True(del2Resp.IsSuccess);
            }
        }

        private static async Task<int> DeleteAllObjects(IObjectClient client, string bucket)
        {
            ListObjectVersionsResponse response;
            Task<ListObjectVersionsResponse> responseTask = client.ListObjectVersionsAsync(bucket, req => req.KeyMarker = null);

            int failed = 0;

            do
            {
                response = await responseTask;

                if (!response.IsSuccess)
                    return -1;

                if (response.Versions.Count + response.DeleteMarkers.Count == 0)
                    break;

                string keyMarker = response.NextKeyMarker;
                responseTask = client.ListObjectVersionsAsync(bucket, req => req.KeyMarker = keyMarker);

                IEnumerable<S3DeleteInfo> delete = response.Versions.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId))
                                                           .Concat(response.DeleteMarkers.Select(x => new S3DeleteInfo(x.ObjectKey, x.VersionId)));

                DeleteObjectsResponse multiDelResponse = await client.DeleteObjectsAsync(bucket, delete, req => req.Quiet = false).ConfigureAwait(false);

                if (!multiDelResponse.IsSuccess)
                    return -1;

                failed += multiDelResponse.Errors.Count;

            } while (response.IsTruncated);

            return failed;
        }
    }
}