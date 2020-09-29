using System;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Add SimpleS3 services to a service collection.</summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config, IServiceProvider> config)
        {
            collection.Configure(config);
            return AddSimpleS3(collection);
        }

        /// <summary>Add SimpleS3 services to a service collection.</summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<S3Config> config)
        {
            collection.Configure(config);
            return AddSimpleS3(collection);
        }

        /// <summary>Add SimpleS3 services to a service collection.</summary>
        /// <param name="collection">The service collection</param>
        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection)
        {
            S3ClientBuilder builder = new S3ClientBuilder(collection);

            ICoreBuilder clientBuilder = collection.AddSimpleS3Core();
            clientBuilder.UseS3Client();
            builder.CoreBuilder = clientBuilder;

            IHttpClientBuilder httpBuilder = clientBuilder.UseHttpClientFactory();
            httpBuilder.UseDefaultHttpPolicy();
            builder.HttpBuilder = httpBuilder;

            return builder;
        }
    }
}