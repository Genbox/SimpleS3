using System;
using System.Net;
using System.Net.Http;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config, IServiceProvider> configureS3 = null)
        {
            IClientBuilder builder = collection.AddSimpleS3Core();
            builder.UseS3Client();

            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();
            httpBuilder.AddDefaultHttpPolicy();

            if (configureS3 != null)
                collection.Configure(configureS3);

            return builder;
        }

        public static IClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3)
        {
            return collection.AddSimpleS3((config, provider) => configureS3(config));
        }

        public static IClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3, HttpMessageHandler handler)
        {
            IClientBuilder builder = collection.AddSimpleS3Core();
            builder.UseS3Client();

            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();
            httpBuilder.AddDefaultHttpPolicy();

            if (handler != null)
                httpBuilder.ConfigurePrimaryHttpMessageHandler(() => handler);

            if (configureS3 != null)
                collection.Configure(configureS3);

            return builder;
        }

        public static IClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3, IWebProxy proxy)
        {
            return collection.AddSimpleS3(configureS3, new HttpClientHandler { Proxy = proxy });
        }
    }
}