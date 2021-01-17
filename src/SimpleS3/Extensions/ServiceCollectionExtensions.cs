using System;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Extensions.AwsS3;
using Genbox.SimpleS3.Extensions.AwsS3.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Polly.Extensions;
using Genbox.SimpleS3.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Add SimpleS3 services to a service collection.</summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<AwsConfig, IServiceProvider> config)
        {
            collection.Configure(config);
            return AddSimpleS3(collection);
        }

        /// <summary>Add SimpleS3 services to a service collection.</summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection, Action<AwsConfig> config)
        {
            collection.Configure(config);
            return AddSimpleS3(collection);
        }

        /// <summary>Add SimpleS3 services to a service collection.</summary>
        /// <param name="collection">The service collection</param>
        public static IS3ClientBuilder AddSimpleS3(this IServiceCollection collection)
        {
            ICoreBuilder coreBuilder = collection.AddSimpleS3Core();
            coreBuilder.UseAwsS3();
            coreBuilder.UseS3Client();

            IHttpClientBuilder httpBuilder = coreBuilder.UseHttpClientFactory();
            httpBuilder.UseDefaultHttpPolicy();

            return new S3ClientBuilder(collection, httpBuilder, coreBuilder);
        }
    }
}