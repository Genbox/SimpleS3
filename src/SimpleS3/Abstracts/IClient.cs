using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IClient : IBucketClient, IObjectClient, IMultipartClient, ITransfer, IMultipartTransfer { }
}