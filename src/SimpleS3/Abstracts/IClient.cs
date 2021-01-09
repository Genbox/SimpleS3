using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Fluent;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IClient : IBucketClient, IObjectClient, IMultipartClient, IMultipartTransfer { }
}