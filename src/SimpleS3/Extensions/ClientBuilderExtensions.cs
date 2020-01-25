using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions
{
    public static class ClientBuilderExtensions
    {
        public static IClientBuilder UseS3Client(this IClientBuilder builder)
        {
            builder.Services.TryAddSingleton<S3Client>();
            builder.Services.TryAddSingleton<IClient, S3Client>();
            return builder;
        }
    }
}