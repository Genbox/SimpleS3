using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Multipart
{
    public class AbortMultipartUploadResponse : BaseResponse, IHasRequestCharged
    {
        public bool RequestCharged { get; internal set; }
    }
}