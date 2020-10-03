using System;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.ErrorHandling.Status;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Extensions.ProfileManager.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests
{
    public abstract class OnlineTestBase : IDisposable
    {
        private readonly ServiceProvider _services;

        protected OnlineTestBase(ITestOutputHelper outputHelper)
        {
            ConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("Config.json", false);

            //Set the configuration from the config file
            IConfigurationRoot configRoot = configBuilder.Build();

            ServiceCollection collection = new ServiceCollection();

            ICoreBuilder coreBuilder = collection.AddSimpleS3Core(config =>
            {
                //Set the configuration from the config file
                configRoot.Bind(config);
            });

            coreBuilder.UseProfileManager()
                       .BindConfigToDefaultProfile()
                       .UseDataProtection();

            IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
            httpBuilder.UseTimeoutPolicy(TimeSpan.FromMinutes(10));

            IConfigurationSection proxySection = configRoot.GetSection("Proxy");

            if (proxySection != null && proxySection["UseProxy"].Equals("true", StringComparison.OrdinalIgnoreCase))
                httpBuilder.WithProxy(proxySection["ProxyAddress"]);

            collection.AddLogging(x =>
            {
                x.AddConfiguration(configRoot.GetSection("Logging"));
                x.AddXUnit(outputHelper);
            });

            //A small hack to remove all validators, as we test them separately
            collection.RemoveAll(typeof(IValidator<>));
            collection.RemoveAll<IValidator>();

            _services = collection.BuildServiceProvider();

            BucketName = configRoot["BucketName"] ?? "main-test-bucket-2019";

            ObjectClient = _services.GetRequiredService<IObjectClient>();
            BucketClient = _services.GetRequiredService<IBucketClient>();
            MultipartClient = _services.GetRequiredService<IMultipartClient>();
            Transfer = _services.GetRequiredService<Fluent.Transfer>();
            SignedObjectClient = _services.GetRequiredService<ISignedObjectClient>();
        }

        protected string BucketName { get; }
        protected IObjectClient ObjectClient { get; }
        protected IBucketClient BucketClient { get; }
        protected IMultipartClient MultipartClient { get; }
        protected Fluent.Transfer Transfer { get; }
        protected ISignedObjectClient SignedObjectClient { get; }

        public void Dispose()
        {
            _services?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected async Task<PutObjectResponse> UploadAsync(string bucketName, string objectKey, Action<PutObjectRequest>? config = null, bool assumeSuccess = true)
        {
            PutObjectResponse resp = await ObjectClient.PutObjectStringAsync(bucketName, objectKey, "test", Encoding.UTF8, config).ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadAsync(string objectKey, Action<PutObjectRequest>? config = null, bool assumeSuccess = true)
        {
            return UploadAsync(BucketName, objectKey, config, assumeSuccess);
        }

        protected async Task<PutObjectResponse> UploadTransferAsync(string bucketName, string objectKey, Action<Upload>? action = null, bool assumeSuccess = true)
        {
            Upload upload = Transfer.Upload(bucketName, objectKey);
            action?.Invoke(upload);

            PutObjectResponse resp = await upload.UploadStringAsync("test", Encoding.UTF8).ConfigureAwait(false);

            if (assumeSuccess)
                Assert.True(resp.IsSuccess);
            else
                Assert.False(resp.IsSuccess);

            return resp;
        }

        protected Task<PutObjectResponse> UploadTransferAsync(string objectKey, Action<Upload>? action = null, bool assumeSuccess = true)
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

        protected async Task<GetObjectResponse> AssertTransferAsync(string bucketName, string objectKey, Action<Download>? config = null, bool assumeSuccess = true)
        {
            Download download = Transfer.Download(bucketName, objectKey);
            config?.Invoke(download);
            GetObjectResponse resp = await download.DownloadAsync().ConfigureAwait(false);

            if (assumeSuccess)
            {
                Assert.True(resp.IsSuccess);
                Assert.Equal("test", await resp.Content!.AsStringAsync().ConfigureAwait(false));
            }

            return resp;
        }

        protected Task<GetObjectResponse> AssertTransferAsync(string objectKey, Action<Download>? config = null, bool assumeSuccess = true)
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
                DeleteAllObjectsStatus delResp = await ObjectClient.DeleteAllObjectsAsync(tempBucketName).ConfigureAwait(false);
                Assert.Equal(DeleteAllObjectsStatus.Ok, delResp);

                DeleteBucketResponse del2Resp = await BucketClient.DeleteBucketAsync(tempBucketName).ConfigureAwait(false);
                Assert.True(del2Resp.IsSuccess);
            }
        }
    }
}