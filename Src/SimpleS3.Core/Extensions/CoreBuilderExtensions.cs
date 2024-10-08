﻿using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Internals.Clients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Core.Extensions;

public static class CoreBuilderExtensions
{
    public static ICoreBuilder UsePooledClients(this ICoreBuilder builder)
    {
        builder.Services.Replace(ServiceDescriptor.Singleton<IBucketClient, PooledBucketClient>());
        builder.Services.Replace(ServiceDescriptor.Singleton<IObjectClient, PooledObjectClient>());
        builder.Services.Replace(ServiceDescriptor.Singleton<IMultipartClient, PooledMultipartClient>());
        return builder;
    }
}