using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed
{
    public class SignedHeadObjectRequest : SignedBaseRequest
    {
        public SignedHeadObjectRequest(string url) : base(HttpMethod.HEAD, url) { }
    }
}