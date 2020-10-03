using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Fluent;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network;
using Genbox.SimpleS3.Core.Network.RequestWrappers;
using Genbox.SimpleS3.Core.Operations;
using Genbox.SimpleS3.Core.Validation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IValidatorFactory = Genbox.SimpleS3.Core.Abstracts.Factories.IValidatorFactory;

namespace Genbox.SimpleS3.Core.Extensions
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this
        /// method is strictly if you are an advanced user. Use AddSimpleS3() if you need something simple that works.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection, Action<S3Config, IServiceProvider> config)
        {
            collection.Configure(config);
            return AddSimpleS3Core(collection);
        }

        /// <summary>
        /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this
        /// method is strictly if you are an advanced user. Use AddSimpleS3() if you need something simple that works.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="config">The configuration delegate</param>
        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection, Action<S3Config> config)
        {
            collection.Configure(config);
            return AddSimpleS3Core(collection);
        }

        /// <summary>
        /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this
        /// method is strictly if you are an advanced user. Use AddSimpleS3() if you need something simple that works.
        /// </summary>
        /// <param name="collection">The service collection</param>
        public static ICoreBuilder AddSimpleS3Core(this IServiceCollection collection)
        {
            collection.AddLogging();
            collection.AddOptions();

            //Authentication
            collection.TryAddSingleton<ISigningKeyBuilder, SigningKeyBuilder>();
            collection.TryAddSingleton<IScopeBuilder, ScopeBuilder>();
            collection.TryAddSingleton<ISignatureBuilder, SignatureBuilder>();
            collection.TryAddSingleton<IChunkedSignatureBuilder, ChunkedSignatureBuilder>();
            collection.TryAddSingleton<HeaderAuthorizationBuilder>();
            collection.TryAddSingleton<ISignedRequestHandler, DefaultSignedRequestHandler>();
            collection.TryAddSingleton<QueryParameterAuthorizationBuilder>();

            //Operations
            collection.TryAddSingleton<IObjectOperations, ObjectOperations>();
            collection.TryAddSingleton<IBucketOperations, BucketOperations>();
            collection.TryAddSingleton<IMultipartOperations, MultipartOperations>();
            collection.TryAddSingleton<ISignedObjectOperations, SignedObjectOperations>();

            //Clients
            collection.TryAddSingleton<IObjectClient, S3ObjectClient>();
            collection.TryAddSingleton<IBucketClient, S3BucketClient>();
            collection.TryAddSingleton<IMultipartClient, S3MultipartClient>();
            collection.TryAddSingleton<ISignedObjectClient, S3SignedObjectClient>();

            //Misc
            collection.TryAddSingleton<IRequestHandler, DefaultRequestHandler>();
            collection.TryAddSingleton<IValidatorFactory, ValidatorFactory>();
            collection.TryAddSingleton<IMarshalFactory, MarshalFactory>();
            collection.TryAddSingleton<IPostMapperFactory, PostMapperFactory>();

            //Fluent
            collection.TryAddSingleton<Transfer>();

            collection.AddSingleton<IRequestStreamWrapper, ChunkedContentRequestStreamWrapper>(); //This has to be added using AddSingleton

            Assembly assembly = typeof(S3Config).Assembly; //Needs to be the assembly that contains the types

            collection.TryAddEnumerable(CreateRegistrations(typeof(IValidator), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IRequestMarshal), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IResponseMarshal), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IPostMapper), assembly));

            return new CoreBuilder(collection);
        }

        private static IEnumerable<ServiceDescriptor> CreateRegistrations(Type abstractType, Assembly assembly)
        {
            foreach (Type type in TypeHelper.GetInstanceTypesInheritedFrom(abstractType, assembly))
            {
                yield return ServiceDescriptor.Singleton(abstractType, type);
            }
        }
    }
}