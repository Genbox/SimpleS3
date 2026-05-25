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

namespace Genbox.SimpleS3.Core.Extensions;

[PublicAPI]
public static class SimpleS3CoreServices
{
    /// <summary>
    /// Add the SimpleS3 core services to a service collection. Note that it does not add a network driver, profile manager or anything else - this method is strictly if you are
    /// an advanced user. Use AddSimpleS3() if you need something simple that works.
    /// </summary>
    /// <param name="collection">The service collection</param>
    /// <param name="configure">Use this to configure the configuration used by SimpleS3</param>
    /// <param name="name">The HTTP client name to use for named network-driver configuration.</param>
    public static ICoreBuilder AddSimpleS3Core(IServiceCollection collection, Action<SimpleS3Config>? configure = null, string name = ServiceBuilderBase.DefaultName)
    {
        //We don't use the microsoft extension here as we only want a subset of services.
        string optionsName = ServiceBuilderBase.GetOptionsName(name);

        //Add options
        collection.TryAdd(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));
        collection.TryAdd(ServiceDescriptor.Transient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));
        collection.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>)));
        collection.TryAdd(ServiceDescriptor.Singleton(typeof(IOptionsMonitorCache<>), typeof(OptionsCache<>)));
        collection.AddKeyedSingleton<IOptions<SimpleS3Config>>(name, (provider, _) =>
        {
            IOptionsMonitor<SimpleS3Config> options = provider.GetRequiredService<IOptionsMonitor<SimpleS3Config>>();
            return new OptionsWrapper<SimpleS3Config>(options.Get(optionsName));
        });

        //Add logging
        collection.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>());
        collection.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

        //Config
        if (configure != null)
            collection.Configure(optionsName, configure);

        //Authentication
        collection.AddSingleton<ISigningKeyBuilder, SigningKeyBuilder>();
        collection.AddSingleton<IScopeBuilder, ScopeBuilder>();
        collection.AddSingleton<ISignatureBuilder, SignatureBuilder>();
        collection.AddSingleton<IChunkedSignatureBuilder, ChunkedSignatureBuilder>();
        collection.AddSingleton<HeaderAuthorizationBuilder>();
        collection.AddSingleton<QueryParameterAuthorizationBuilder>();

        //Operations
        collection.AddSingleton<IObjectOperations, ObjectOperations>();
        collection.AddSingleton<IBucketOperations, BucketOperations>();
        collection.AddSingleton<IMultipartOperations, MultipartOperations>();
        collection.AddSingleton<ISignedOperations, SignedOperations>();

        //Clients
        collection.AddSingleton<IObjectClient, ObjectClient>();
        collection.AddSingleton<IBucketClient, BucketClient>();
        collection.AddSingleton<IMultipartClient, MultipartClient>();
        collection.AddSingleton<ISignedClient, SignedClient>();
        collection.AddKeyedSingleton<ISimpleClient>(name, (provider, _) => CreateClient(provider, name));

        if (name == ServiceBuilderBase.DefaultName)
            collection.AddSingleton(provider => provider.GetRequiredKeyedService<ISimpleClient>(name));

        //Misc
        collection.AddSingleton<IResponseHandler, DefaultResponseHandler>();
        collection.AddSingleton<IRequestHandler, DefaultRequestHandler>();
        collection.AddSingleton<ISignedRequestHandler, DefaultSignedRequestHandler>();
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

        return new CoreBuilder(collection, name);
    }

    private static SimpleClient CreateClient(IServiceProvider provider, string name)
    {
        IOptions<SimpleS3Config> options = GetRequiredNamedService<IOptions<SimpleS3Config>>(provider, name);
        IInputValidator inputValidator = GetInputValidator(provider, name);
        INetworkDriver networkDriver = GetRequiredNamedService<INetworkDriver>(provider, name);
        IAccessKeyProtector? accessKeyProtector = provider.GetService<IAccessKeyProtector>();
        IMarshalFactory marshalFactory = provider.GetRequiredService<IMarshalFactory>();
        IPostMapperFactory postMapperFactory = provider.GetRequiredService<IPostMapperFactory>();
        ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        ScopeBuilder scopeBuilder = new ScopeBuilder(options);
        SigningKeyBuilder signingKeyBuilder = new SigningKeyBuilder(options, loggerFactory.CreateLogger<SigningKeyBuilder>(), accessKeyProtector);
        SignatureBuilder signatureBuilder = new SignatureBuilder(signingKeyBuilder, scopeBuilder, options, loggerFactory.CreateLogger<SignatureBuilder>());
        ChunkedSignatureBuilder chunkedSignatureBuilder = new ChunkedSignatureBuilder(signingKeyBuilder, scopeBuilder, options, loggerFactory.CreateLogger<ChunkedSignatureBuilder>());
        HeaderAuthorizationBuilder headerAuthorizationBuilder = new HeaderAuthorizationBuilder(options, scopeBuilder, signatureBuilder, loggerFactory.CreateLogger<HeaderAuthorizationBuilder>());
        QueryParameterAuthorizationBuilder queryParameterAuthorizationBuilder = new QueryParameterAuthorizationBuilder(signatureBuilder, loggerFactory.CreateLogger<QueryParameterAuthorizationBuilder>());

        ValidatorFactory validatorFactory = ValidatorFactory.Create(provider, options, inputValidator);
        DefaultResponseHandler responseHandler = new DefaultResponseHandler(options, validatorFactory, marshalFactory, postMapperFactory, networkDriver, loggerFactory.CreateLogger<DefaultResponseHandler>());
        EndpointBuilder endpointBuilder = new EndpointBuilder(options);

        IEnumerable<IRequestStreamWrapper> requestStreamWrappers = new IRequestStreamWrapper[] { new ChunkedContentRequestStreamWrapper(options, chunkedSignatureBuilder, signatureBuilder) }
            .Concat(provider.GetKeyedServices<IRequestStreamWrapper>(name));

        DefaultRequestHandler requestHandler = new DefaultRequestHandler(options, validatorFactory, marshalFactory, responseHandler, headerAuthorizationBuilder, endpointBuilder, loggerFactory.CreateLogger<DefaultRequestHandler>(), requestStreamWrappers);
        DefaultSignedRequestHandler signedRequestHandler = new DefaultSignedRequestHandler(options, marshalFactory, scopeBuilder, queryParameterAuthorizationBuilder, endpointBuilder, responseHandler, loggerFactory.CreateLogger<DefaultSignedRequestHandler>());

        ObjectOperations objectOperations = new ObjectOperations(requestHandler, provider.GetServices<IRequestWrapper>(), provider.GetServices<IResponseWrapper>());
        BucketOperations bucketOperations = new BucketOperations(requestHandler);
        MultipartOperations multipartOperations = new MultipartOperations(requestHandler, provider.GetServices<IRequestWrapper>(), provider.GetServices<IResponseWrapper>());
        SignedOperations signedOperations = new SignedOperations(signedRequestHandler);

        ObjectClient objectClient = new ObjectClient(objectOperations);
        BucketClient bucketClient = new BucketClient(bucketOperations);
        MultipartClient multipartClient = new MultipartClient(multipartOperations);
        SignedClient signedClient = new SignedClient(signedOperations);
        MultipartTransfer multipartTransfer = new MultipartTransfer(objectClient, multipartClient, multipartOperations, provider.GetServices<IRequestWrapper>());
        Transfer transfer = new Transfer(objectOperations, multipartTransfer);

        return new SimpleClient(objectClient, bucketClient, multipartClient, multipartTransfer, transfer, signedClient);
    }

    private static T GetRequiredNamedService<T>(IServiceProvider provider, string name) where T : class
    {
        T? service = GetNamedService<T>(provider, name);

        if (service != null)
            return service;

        throw new InvalidOperationException($"No service for type '{typeof(T)}' and name '{name}' has been registered.");
    }

    private static T? GetNamedService<T>(IServiceProvider provider, string name) where T : class
    {
        T? service = provider.GetKeyedService<T>(name);

        if (service != null)
            return service;

        return name == ServiceBuilderBase.DefaultName ? provider.GetService<T>() : null;
    }

    private static IInputValidator GetInputValidator(IServiceProvider provider, string name)
    {
        if (name == ServiceBuilderBase.DefaultName)
            return provider.GetService<IInputValidator>() ?? provider.GetKeyedService<IInputValidator>(name) ?? new NullInputValidator();

        return provider.GetKeyedService<IInputValidator>(name) ?? new NullInputValidator();
    }

    /// <summary>Register services as the interface type given</summary>
    private static IEnumerable<ServiceDescriptor> RegisterAs(Type abstractType, Assembly assembly)
    {
        foreach (Type type in TypeHelper.GetInstanceTypesInheritedFrom(abstractType, assembly))
            yield return ServiceDescriptor.Singleton(abstractType, type);
    }

    /// <summary>Register a service that inherits from an open generic (e.g. Service&lt;&gt;) as the actual implementation (e.g. Service&lt;T&gt;)</summary>
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