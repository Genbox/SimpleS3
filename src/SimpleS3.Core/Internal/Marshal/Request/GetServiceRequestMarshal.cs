using System.IO;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Requests.Service;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class GetServiceRequestMarshal : IRequestMarshal<GetServiceRequest>
    {
        public Stream MarshalRequest(GetServiceRequest request)
        {
            return null;
        }
    }
}