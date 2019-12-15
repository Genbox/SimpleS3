using System;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.LiveTests
{
    public abstract class LiveTestBase : IDisposable
    {
        private readonly IConfigurationRoot _configRoot;

        protected LiveTestBase(ITestOutputHelper outputHelper)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("TestConfig.json", false);

            ServiceCollection collection = new ServiceCollection();

            //Set the configuration from the config file
            configBuilder.AddUserSecrets(GetType().Assembly);
            _configRoot = configBuilder.Build();

            IS3ClientBuilder builder = collection.AddSimpleS3Core(ConfigureS3);
            IHttpClientBuilder httpClientBuilder = builder.UseHttpClientFactory();

            IConfigurationSection proxySection = _configRoot.GetSection("Proxy");

            if (proxySection != null && proxySection["UseProxy"].Equals("true", StringComparison.OrdinalIgnoreCase))
                httpClientBuilder.WithProxy(proxySection["ProxyAddress"]);

            collection.AddLogging(x =>
            {
                x.AddConfiguration(_configRoot.GetSection("Logging"));
                x.AddXUnit(outputHelper);
            });

            //A small hack to remove all validators, as we test them separately
            collection.RemoveAll(typeof(IValidator<>));
            collection.RemoveAll<IValidator>();

            Services = collection.BuildServiceProvider();

            BucketName = _configRoot["BucketName"] ?? "main-test-bucket-2019";

            Config = Services.GetRequiredService<IOptions<S3Config>>().Value;
            ObjectClient = Services.GetRequiredService<IS3ObjectClient>();
            BucketClient = Services.GetRequiredService<IS3BucketClient>();
            MultipartClient = Services.GetRequiredService<IS3MultipartClient>();
            Transfer = Services.GetRequiredService<Core.Fluent.Transfer>();

            //foreach (S3Bucket bucket in BucketClient.ListAllBucketsAsync().ToListAsync().Result)
            //{
            //    if (bucket.Name.Contains("test", StringComparison.OrdinalIgnoreCase))
            //        BucketClient.DeleteBucketRecursiveAsync(bucket.Name).Wait();
            //}
        }

        public ServiceProvider Services { get; }

        protected S3Config Config { get; }
        protected string BucketName { get; }
        protected IS3ObjectClient ObjectClient { get; }
        protected IS3BucketClient BucketClient { get; }
        protected IS3MultipartClient MultipartClient { get; }
        protected Core.Fluent.Transfer Transfer { get; }

        public void Dispose()
        {
            Services?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void ConfigureS3(S3Config config)
        {
            //Set the configuration from the config file
            _configRoot.Bind(config);

            StringAccessKey secret = new StringAccessKey(_configRoot[nameof(StringAccessKey.KeyId)], _configRoot[nameof(StringAccessKey.AccessKey)]);

            if (string.IsNullOrWhiteSpace(secret.KeyId))
                throw new Exception("Did you forget to set a KeyId? See Readme.txt on how to run live tests");

            if (secret.AccessKey == null)
                throw new Exception("Did you forget to set a secret key? See Readme.txt on how to run live tests");

            config.Credentials = secret;
        }

        protected async Task<PutObjectResponse> UploadAsync(string bucketName, string objectKey, Action<PutObjectRequest> config = null, bool assumeSuccess = true)
        {
            PutObjectResponse resp = await ObjectClient.PutObjectStringAsync(bucketName, objectKey, "test", Encoding.UTF8, config).ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadAsync(string objectKey, Action<PutObjectRequest> config = null, bool assumeSuccess = true)
        {
            return UploadAsync(BucketName, objectKey, config, assumeSuccess);
        }

        protected async Task<PutObjectResponse> UploadTransferAsync(string bucketName, string objectKey, Action<Upload> action = null, bool assumeSuccess = true)
        {
            Upload upload = Transfer.UploadString(bucketName, objectKey, "test", Encoding.UTF8);
            action?.Invoke(upload);

            PutObjectResponse resp = await upload.ExecuteAsync().ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadTransferAsync(string objectKey, Action<Upload> action = null, bool assumeSuccess = true)
        {
            return UploadTransferAsync(BucketName, objectKey, action, assumeSuccess);
        }

        protected async Task<GetObjectResponse> AssertAsync(string bucketName, string objectKey, Action<GetObjectRequest> config = null, bool assumeSuccess = true)
        {
            GetObjectResponse resp = await ObjectClient.GetObjectAsync(bucketName, objectKey, config).ConfigureAwait(false);

            if (assumeSuccess)
            {
                Assert.True(resp.IsSuccess);
                Assert.Equal("test", await resp.Content.AsStringAsync().ConfigureAwait(false));
            }

            return resp;
        }

        protected Task<GetObjectResponse> AssertAsync(string objectKey, Action<GetObjectRequest> config = null, bool assumeSuccess = true)
        {
            return AssertAsync(BucketName, objectKey, config, assumeSuccess);
        }

        protected async Task<GetObjectResponse> AssertTransferAsync(string bucketName, string objectKey, Action<Download> config = null, bool assumeSuccess = true)
        {
            Download download = Transfer.Download(bucketName, objectKey);
            config?.Invoke(download);
            GetObjectResponse resp = await download.ExecuteAsync().ConfigureAwait(false);

            if (assumeSuccess)
            {
                Assert.True(resp.IsSuccess);
                Assert.Equal("test", await resp.Content.AsStringAsync().ConfigureAwait(false));
            }

            return resp;
        }

        protected Task<GetObjectResponse> AssertTransferAsync(string objectKey, Action<Download> config = null, bool assumeSuccess = true)
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
                await (action?.Invoke(tempBucketName)).ConfigureAwait(false);
            }
            finally
            {
                DeleteAllObjectsStatus delResp = await ObjectClient.DeleteAllObjectsAsync(tempBucketName).ConfigureAwait(false);
                Assert.Equal(DeleteAllObjectsStatus.Ok, delResp);

                DeleteBucketResponse del2Resp = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
                Assert.True(del2Resp.IsSuccess);
            }
        }
    }
}