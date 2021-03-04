using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Transfer;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface ISimpleClient : IBucketClient, IObjectClient, IMultipartClient, ITransfer, IMultipartTransfer { }
}