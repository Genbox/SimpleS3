using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Extensions
{
    public static class CoreBuilderExtensions
    {
        public static ICoreBuilder UseS3Client(this ICoreBuilder builder)
        {
            builder.UseTransfer();

            builder.Services.AddSingleton(x =>
            {
                //We have to call a specific constructor for dependency injection
                IObjectClient objectClient = x.GetRequiredService<IObjectClient>();
                IBucketClient bucketClient = x.GetRequiredService<IBucketClient>();
                IMultipartClient multipartClient = x.GetRequiredService<IMultipartClient>();
                Transfer transfer = x.GetRequiredService<Transfer>();
                return new S3Client(objectClient, bucketClient, multipartClient, transfer);
            });

            //Add the client as the interface too
            builder.Services.AddSingleton<IClient>(x => x.GetRequiredService<S3Client>());
            return builder;
        }
    }
}