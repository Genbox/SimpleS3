using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network;
using Genbox.SimpleS3.Core.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseS3Client(this ICoreBuilder builder)
        {
            builder.Services.TryAddSingleton(x =>
            {
                //We have to call a specific constructor for dependency injection
                IObjectClient objectClient = x.GetRequiredService<IObjectClient>();
                IBucketClient bucketClient = x.GetRequiredService<IBucketClient>();
                IMultipartClient multipartClient = x.GetRequiredService<IMultipartClient>();
                return new S3Client(objectClient, bucketClient, multipartClient);
            });

            //Add the client as the interface too
            builder.Services.TryAddSingleton<IClient>(x => x.GetRequiredService<S3Client>());
            return builder;
        }


        public static ICoreBuilder UsePreSigned(this ICoreBuilder builder)
        {
            builder.Services.TryAddSingleton<IPreSignedObjectOperations, PreSignedObjectOperations>();
            builder.Services.TryAddSingleton<IPreSignRequestHandler, DefaultPreSignedRequestHandler>();
            return builder;
        }
    }
}