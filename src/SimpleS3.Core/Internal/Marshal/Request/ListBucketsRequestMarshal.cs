using System.IO;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Requests.Buckets;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class ListBucketsRequestMarshal : IRequestMarshal<ListBucketsRequest>
    {
        public Stream MarshalRequest(ListBucketsRequest request)
        {
            return null;
        }
    }
}