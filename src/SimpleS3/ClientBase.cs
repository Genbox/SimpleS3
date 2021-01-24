using System;
using System.Net;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3
{
    /// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
    public abstract class ClientBase : SimpleS3Client, IDisposable
    {
        private readonly ServiceProvider? _serviceProvider;

        /// <summary>Creates a new instance of <see cref="ClientBase" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        protected ClientBase(string keyId, byte[] accessKey, string region, IWebProxy? proxy = null) : this(new Config(new AccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="ClientBase" /></summary>
        /// <param name="keyId">The key id</param>
        /// <param name="accessKey">The secret access key</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        protected ClientBase(string keyId, string accessKey, string region, IWebProxy? proxy = null) : this(new Config(new StringAccessKey(keyId, accessKey), region), proxy) { }

        /// <summary>Creates a new instance of <see cref="ClientBase" /></summary>
        /// <param name="credentials">The credentials to use</param>
        /// <param name="region">The region you wish to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        protected ClientBase(IAccessKey credentials, string region, IWebProxy? proxy = null) : this(new Config(credentials, region), proxy) { }

        /// <summary>Creates a new instance of <see cref="ClientBase" /></summary>
        /// <param name="config">The configuration you want to use</param>
        /// <param name="proxy">A web proxy (optional)</param>
        protected ClientBase(Config config, IWebProxy? proxy = null)
        {
            ServiceCollection services = new ServiceCollection();
            ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(services);

            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();

            if (proxy != null)
                httpBuilder.UseProxy(proxy);

            services.AddSingleton(Options.Create(config));

            _serviceProvider = services.BuildServiceProvider();

            IObjectClient objectClient = _serviceProvider.GetRequiredService<IObjectClient>();
            IBucketClient bucketClient = _serviceProvider.GetRequiredService<IBucketClient>();
            IMultipartClient multipartClient = _serviceProvider.GetRequiredService<IMultipartClient>();
            IMultipartTransfer multipartTransfer = _serviceProvider.GetRequiredService<IMultipartTransfer>();
            ITransfer transfer = _serviceProvider.GetRequiredService<ITransfer>();

            Initialize(objectClient, bucketClient, multipartClient, multipartTransfer, transfer);
        }

        protected ClientBase(Config config, INetworkDriver networkDriver)
        {
            ServiceCollection services = new ServiceCollection();
            SimpleS3CoreServices.AddSimpleS3Core(services);

            services.AddSingleton(networkDriver);
            services.AddSingleton(Options.Create(config));

            _serviceProvider = services.BuildServiceProvider();

            IObjectClient objectClient = _serviceProvider.GetRequiredService<IObjectClient>();
            IBucketClient bucketClient = _serviceProvider.GetRequiredService<IBucketClient>();
            IMultipartClient multipartClient = _serviceProvider.GetRequiredService<IMultipartClient>();
            IMultipartTransfer multipartTransfer = _serviceProvider.GetRequiredService<IMultipartTransfer>();
            ITransfer transfer = _serviceProvider.GetRequiredService<ITransfer>();

            Initialize(objectClient, bucketClient, multipartClient, multipartTransfer, transfer);
        }

        protected ClientBase(IObjectClient objectClient, IBucketClient bucketClient, IMultipartClient multipartClient, IMultipartTransfer multipartTransfer, ITransfer transfer)
        {
            Initialize(objectClient, bucketClient, multipartClient, multipartTransfer, transfer);
        }

        protected virtual void Dispose(bool disposing)
        {
            _serviceProvider?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}