using System;
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
            S3ClientBuilder builder = new S3ClientBuilder(collection);

            ICoreBuilder clientBuilder = collection.AddSimpleS3Core();
            clientBuilder.UseS3Client();
            builder.CoreBuilder = clientBuilder;

            IHttpClientBuilder httpBuilder = clientBuilder.UseHttpClientFactory();
            httpBuilder.UseDefaultHttpPolicy();
            builder.HttpBuilder = httpBuilder;

            if (configureS3 != null)
                collection.Configure(configureS3);

            return builder;
        }

        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> configureS3)
        {
            S3ClientBuilder builder = new S3ClientBuilder(collection);

            ICoreBuilder clientBuilder = collection.AddSimpleS3Core();
            clientBuilder.UseS3Client();
            builder.CoreBuilder = clientBuilder;

            IHttpClientBuilder httpBuilder = clientBuilder.UseHttpClientFactory();
            httpBuilder.UseDefaultHttpPolicy();
            builder.HttpBuilder = httpBuilder;

            if (configureS3 != null)
                collection.Configure(configureS3);

            return builder;
        }
    }
}