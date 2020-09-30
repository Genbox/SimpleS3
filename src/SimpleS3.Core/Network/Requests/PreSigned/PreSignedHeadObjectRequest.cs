using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.PreSigned
{
    public class PreSignedHeadObjectRequest : PreSignedBaseRequest
    {
        public PreSignedHeadObjectRequest(string url) : base(HttpMethod.HEAD, url) { }
    }
}