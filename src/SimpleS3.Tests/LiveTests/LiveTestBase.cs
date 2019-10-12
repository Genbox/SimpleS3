using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Fluid;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Responses.Buckets;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Extensions.HttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests
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
                httpClientBuilder.ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler {Proxy = new WebProxy(proxySection["ProxyAddress"])});

            collection.AddLogging(x =>
            {
                x.AddConfiguration(_configRoot.GetSection("Logging"));
                x.AddXUnit(outputHelper);
            });

            //A small hack to remove all validators, as we test them separately
            collection.RemoveAll(typeof(IValidator<>));
            collection.RemoveAll<IValidator>();

            Services = collection.BuildServiceProvider();

            //var _bucketClient = Services.GetRequiredService<IS3BucketClient>();

            //var serviceClient = Services.GetRequiredService<IS3ServiceClient>();

            //var enumerator = serviceClient.GetAllAsync().ToListAsync().Result;

            //foreach (S3Bucket bucket in enumerator)
            //{
            //    if (bucket.Name.Contains("test", StringComparison.OrdinalIgnoreCase))
            //        _bucketClient.DeleteBucketRecursiveAsync(bucket.Name).Wait();
            //}

            BucketName = _configRoot["BucketName"] ?? "main-test-bucket-2019";

            Config = Services.GetRequiredService<IOptions<S3Config>>().Value;
            ObjectClient = Services.GetRequiredService<IS3ObjectClient>();
            BucketClient = Services.GetRequiredService<IS3BucketClient>();
            Transfer = Services.GetRequiredService<Transfer>();
        }

        public ServiceProvider Services { get; }

        protected S3Config Config { get; }
        protected string BucketName { get; }
        protected IS3ObjectClient ObjectClient { get; }
        protected IS3BucketClient BucketClient { get; }
        protected Transfer Transfer { get; }

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

        protected async Task<PutObjectResponse> UploadAsync(string bucketName, string resource, Action<PutObjectRequest> config = null, bool assumeSuccess = true)
        {
            PutObjectResponse resp = await ObjectClient.PutObjectStringAsync(bucketName, resource, "test", Encoding.UTF8, config).ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadAsync(string resource, Action<PutObjectRequest> config = null, bool assumeSuccess = true)
        {
            return UploadAsync(BucketName, resource, config, assumeSuccess);
        }

        protected async Task<PutObjectResponse> UploadTransferAsync(string bucketName, string resource, Action<Upload> action = null, bool assumeSuccess = true)
        {
            Upload upload = Transfer.UploadString(bucketName, resource, "test", Encoding.UTF8);
            action?.Invoke(upload);

            PutObjectResponse resp = await upload.ExecuteAsync().ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadTransferAsync(string resource, Action<Upload> action = null, bool assumeSuccess = true)
        {
            return UploadTransferAsync(BucketName, resource, action, assumeSuccess);
        }

        protected async Task<GetObjectResponse> AssertAsync(string bucketName, string resource, Action<GetObjectRequest> config = null, bool assumeSuccess = true)
        {
            GetObjectResponse resp = await ObjectClient.GetObjectAsync(bucketName, resource, config).ConfigureAwait(false);

            if (assumeSuccess)
            {
                Assert.True(resp.IsSuccess);
                Assert.Equal("test", await resp.Content.AsStringAsync().ConfigureAwait(false));
            }

            return resp;
        }

        protected Task<GetObjectResponse> AssertAsync(string resource, Action<GetObjectRequest> config = null, bool assumeSuccess = true)
        {
            return AssertAsync(BucketName, resource, config, assumeSuccess);
        }

        protected async Task<GetObjectResponse> AssertTransferAsync(string bucketName, string resource, Action<Download> config = null, bool assumeSuccess = true)
        {
            Download download = Transfer.Download(bucketName, resource);
            config?.Invoke(download);
            GetObjectResponse resp = await download.ExecuteAsync().ConfigureAwait(false);

            if (assumeSuccess)
            {
                Assert.True(resp.IsSuccess);
                Assert.Equal("test", await resp.Content.AsStringAsync().ConfigureAwait(false));
            }

            return resp;
        }

        protected Task<GetObjectResponse> AssertTransferAsync(string resource, Action<Download> config = null, bool assumeSuccess = true)
        {
            return AssertTransferAsync(BucketName, resource, config, assumeSuccess);
        }

        protected async Task CreateTempBucketAsync(Func<string, Task> action)
        {
            string tempBucketName = "testbucket-" + Guid.NewGuid();

            CreateBucketResponse createResponse = await BucketClient.CreateBucketAsync(tempBucketName, request => request.Region = Config.Region).ConfigureAwait(false);
            Assert.True(createResponse.IsSuccess);

            try
            {
                await (action?.Invoke(tempBucketName)).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }

            DeleteBucketStatus errorCode = await BucketClient.DeleteBucketRecursiveAsync(tempBucketName).ConfigureAwait(false);
            Assert.Equal(DeleteBucketStatus.Ok, errorCode);
        }
    }
}