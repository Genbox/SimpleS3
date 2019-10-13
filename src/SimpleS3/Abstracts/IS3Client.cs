using Genbox.SimpleS3.Core.Abstracts.Clients;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IS3Client : IS3BucketClient, IS3ObjectClient
    {
    }
}