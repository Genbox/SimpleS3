using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Internals;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Builders;
using Genbox.SimpleS3.Core.Internals.Clients;
using Genbox.SimpleS3.Core.Internals.Fluent;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Internals.Operations;
using Genbox.SimpleS3.Core.Internals.Validation;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Extensions
{
    [PublicAPI]
    public static class SimpleS3CoreServices
    {
        /// <summary>
        /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this
        /// method is strictly if you are an advanced user. Use AddSimpleS3() if you need something simple that works.
        /// </summary>
        /// <param name="collection">The service collection</param>
        /// <param name="configure">Use this to configure the configuration used by SimpleS3</param>
        public static ICoreBuilder AddSimpleS3Core(IServiceCollection collection, Action<Config>? configure = null)
        {
            //This is in place of collection.AddOptions();
            collection.TryAdd(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));
            collection.TryAdd(ServiceDescriptor.Transient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));

            //This is in place of collection.AddLogging()
            collection.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
            collection.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            //Config
            if (configure != null)
                collection.Configure(configure);

            //Authentication
            collection.AddSingleton<ISigningKeyBuilder, SigningKeyBuilder>();
            collection.AddSingleton<IScopeBuilder, ScopeBuilder>();
            collection.AddSingleton<ISignatureBuilder, SignatureBuilder>();
            collection.AddSingleton<IChunkedSignatureBuilder, ChunkedSignatureBuilder>();
            collection.AddSingleton<HeaderAuthorizationBuilder>();
            collection.AddSingleton<ISignedRequestHandler, DefaultSignedRequestHandler>();
            collection.AddSingleton<QueryParameterAuthorizationBuilder>();

            //Operations
            collection.AddSingleton<IObjectOperations, ObjectOperations>();
            collection.AddSingleton<IBucketOperations, BucketOperations>();
            collection.AddSingleton<IMultipartOperations, MultipartOperations>();
            collection.AddSingleton<ISignedObjectOperations, SignedObjectOperations>();

            //Clients
            collection.AddSingleton<IObjectClient, ObjectClient>();
            collection.AddSingleton<IBucketClient, BucketClient>();
            collection.AddSingleton<IMultipartClient, MultipartClient>();
            collection.AddSingleton<ISignedObjectClient, SignedObjectClient>();
            collection.AddSingleton<ISimpleClient, SimpleClient>();

            //Misc
            collection.AddSingleton<IRequestHandler, DefaultRequestHandler>();
            collection.AddSingleton<IRequestValidatorFactory, ValidatorFactory>();
            collection.AddSingleton<IMarshalFactory, MarshalFactory>();
            collection.AddSingleton<IPostMapperFactory, PostMapperFactory>();
            collection.AddSingleton<IRequestStreamWrapper, ChunkedContentRequestStreamWrapper>();
            collection.AddSingleton<IRegionConverter, RegionConverter>();
            collection.TryAddSingleton<IEndpointBuilder, EndpointBuilder>();

            //Fluent
            collection.AddSingleton<ITransfer, Transfer>();
            collection.AddSingleton<IMultipartTransfer, MultipartTransfer>();

            //Default services
            collection.TryAddSingleton<IInputValidator, NullInputValidator>();

            Assembly assembly = typeof(SimpleS3CoreServices).Assembly; //Needs to be the assembly that contains the types

            collection.TryAddEnumerable(RegisterAs(typeof(IRequestMarshal), assembly));
            collection.TryAddEnumerable(RegisterAs(typeof(IResponseMarshal), assembly));
            collection.TryAddEnumerable(RegisterAs(typeof(IPostMapper), assembly));
            collection.TryAddEnumerable(RegisterAs(typeof(IValidator), assembly));
            collection.TryAddEnumerable(RegisterAsActual(typeof(IValidator<>), assembly)); //We register IValidator twice to support IValidator<T> as well
            collection.TryAddEnumerable(RegisterAsActual(typeof(IValidateOptions<>), assembly)); //Make sure that the options system validators are added too

            return new CoreBuilder(collection);
        }

        /// <summary>
        /// Register services as the interface type given
        /// </summary>
        private static IEnumerable<ServiceDescriptor> RegisterAs(Type abstractType, Assembly assembly)
        {
            foreach (Type type in TypeHelper.GetInstanceTypesInheritedFrom(abstractType, assembly))
            {
                yield return ServiceDescriptor.Singleton(abstractType, type);
            }
        }

        /// <summary>
        /// Register a service that inherits from an open generic (e.g. Service&lt;&gt;) as the actual implementation (e.g. Service&lt;T&gt;)
        /// </summary>
        private static IEnumerable<ServiceDescriptor> RegisterAsActual(Type abstractType, Assembly assembly)
        {
            foreach (Type type in TypeHelper.GetInstanceTypesInheritedFrom(abstractType, assembly))
            {
                Type[] interfaceTypes = type.GetInterfaces();

                foreach (Type iType in interfaceTypes)
                {
                    if (iType.IsGenericType && iType.GetGenericTypeDefinition() == abstractType)
                    {
                        yield return ServiceDescriptor.Singleton(iType, type);
                        break;
                    }
                }
            }
        }
    }
}