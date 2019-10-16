using Genbox.SimpleS3.Abstracts;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions
{
    public static class S3ClientBuilderExtensions
    {
        public static IS3ClientBuilder UseS3Client(this IS3ClientBuilder builder)
        {
            builder.Services.TryAddSingleton<S3Client>();
            builder.Services.TryAddSingleton<IS3Client, S3Client>();
            return builder;
        }
    }
}