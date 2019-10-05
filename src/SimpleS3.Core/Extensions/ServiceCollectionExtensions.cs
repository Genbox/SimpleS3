using System;
using FluentValidation;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Abstracts.Factories;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Fluid;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Operations;
using Genbox.SimpleS3.Core.Requests;
using Genbox.SimpleS3.Core.RequestWrappers;
using Genbox.SimpleS3.Core.Validation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using IValidatorFactory = Genbox.SimpleS3.Abstracts.Factories.IValidatorFactory;

namespace Genbox.SimpleS3.Core.Extensions
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        public static IS3ClientBuilder AddSimpleS3Core(this IServiceCollection collection, Action<S3Config, IServiceProvider> configureS3)
        {
            collection?.Configure(configureS3);
            return AddSimpleS3Core(collection);
        }

        public static IS3ClientBuilder AddSimpleS3Core(this IServiceCollection collection, Action<S3Config> configureS3)
        {
            collection?.Configure(configureS3);
            return AddSimpleS3Core(collection);
        }

        public static IS3ClientBuilder AddSimpleS3Core(this IServiceCollection collection)
        {
            collection.AddOptions();
            collection.TryAddSingleton<ISigningKeyBuilder, SigningKeyBuilder>();
            collection.TryAddSingleton<IScopeBuilder, ScopeBuilder>();
            collection.TryAddSingleton<ISignatureBuilder, SignatureBuilder>();
            collection.TryAddSingleton<IChunkedSignatureBuilder, ChunkedSignatureBuilder>();
            collection.TryAddSingleton<IRequestStreamWrapper, ChunkedRequestStreamWrapper>();
            collection.TryAddSingleton<IAuthorizationBuilder, AuthorizationHeaderBuilder>();
            collection.TryAddSingleton<IObjectOperations, ObjectOperations>();
            collection.TryAddSingleton<IBucketOperations, BucketOperations>();
            collection.TryAddSingleton<IServiceOperations, ServiceOperations>();
            collection.TryAddSingleton<IS3ObjectClient, S3ObjectClient>();
            collection.TryAddSingleton<IS3BucketClient, S3BucketClient>();
            collection.TryAddSingleton<IS3ServiceClient, S3ServiceClient>();
            collection.TryAddSingleton<IRequestHandler, DefaultRequestHandler>();
            collection.TryAddSingleton<IS3ConfigCredentialProvider, S3ConfigNullCredentialProvider>();
            collection.TryAddSingleton<IValidatorFactory, ValidatorFactory>();
            collection.TryAddSingleton<IMarshalFactory, MarshalFactory>();
            collection.TryAddSingleton<Transfer>();

            collection.Scan(scan => scan.FromAssemblyOf<S3Config>()
                .AddClasses(x => x.AssignableTo(typeof(IValidator<>)))
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

            collection.Scan(scan => scan.FromAssemblyOf<S3Config>()
                .AddClasses(x => x.AssignableTo(typeof(IRequestMarshal)))
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

            collection.Scan(scan => scan.FromAssemblyOf<S3Config>()
                .AddClasses(x => x.AssignableTo(typeof(IResponseMarshal)))
                .AsSelfWithInterfaces()
                .WithSingletonLifetime());

            return new S3ClientBuilder(collection);
        }

        private static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
        {
            return services.Configure(Options.DefaultName, configureOptions);
        }

        private static IServiceCollection Configure<TOptions>(this IServiceCollection services, string name, Action<TOptions, IServiceProvider> configureOptions) where TOptions : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            services.AddOptions();
            services.AddSingleton<IConfigureOptions<TOptions>>(x => new ConfigureNamedOptions<TOptions, IServiceProvider>(name, x, configureOptions));
            return services;
        }
    }
}