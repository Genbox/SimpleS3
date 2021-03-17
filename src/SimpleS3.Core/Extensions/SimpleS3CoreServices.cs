using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Operations;
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
using IValidatorFactory = Genbox.SimpleS3.Core.Abstracts.Factories.IValidatorFactory;

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
        public static ICoreBuilder AddSimpleS3Core(IServiceCollection collection)
        {
            collection.AddOptions();

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
            collection.AddSingleton<IValidatorFactory, ValidatorFactory>();
            collection.AddSingleton<IMarshalFactory, MarshalFactory>();
            collection.AddSingleton<IPostMapperFactory, PostMapperFactory>();
            collection.AddSingleton<IRequestStreamWrapper, ChunkedContentRequestStreamWrapper>();
            collection.AddSingleton<IRegionConverter, RegionConverter>();

            //Fluent
            collection.AddSingleton<ITransfer, Transfer>();
            collection.AddSingleton<IMultipartTransfer, MultipartTransfer>();

            Assembly assembly = typeof(SimpleS3CoreServices).Assembly; //Needs to be the assembly that contains the types

            collection.TryAddEnumerable(CreateRegistrations(typeof(IRequestMarshal), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IResponseMarshal), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IPostMapper), assembly));
            collection.TryAddEnumerable(CreateRegistrations(typeof(IValidator), assembly));

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