using System;
using System.Net;
using System.Net.Http;
using Genbox.SimpleS3.Abstracts;
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
        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config, IServiceProvider> configureS3 = null)
        {
            IS3ClientBuilder builder = collection.AddSimpleS3Core();
            builder.UseS3Client();

            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();
            httpBuilder.AddDefaultRetryPolicy();

            if (configureS3 != null)
                collection.Configure(configureS3);

            return builder;
        }

        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3)
        {
            return collection.AddSimpleS3((config, provider) => configureS3(config));
        }

        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3, HttpMessageHandler handler)
        {
            IS3ClientBuilder builder = collection.AddSimpleS3Core();
            builder.UseS3Client();

            IHttpClientBuilder httpBuilder = builder.UseHttpClientFactory();
            httpBuilder.AddDefaultRetryPolicy();

            if (handler != null)
                httpBuilder.ConfigurePrimaryHttpMessageHandler(() => handler);

            if (configureS3 != null)
                collection.Configure(configureS3);

            return builder;
        }

        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3, IWebProxy proxy)
        {
            return collection.AddSimpleS3(configureS3, new HttpClientHandler { Proxy = proxy });
        }
    }
}