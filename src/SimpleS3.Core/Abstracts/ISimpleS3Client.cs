using Genbox.SimpleS3.Core.Abstracts.Clients;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface ISimpleS3Client : IBucketClient, IObjectClient, IMultipartClient, ITransfer, IMultipartTransfer { }
}