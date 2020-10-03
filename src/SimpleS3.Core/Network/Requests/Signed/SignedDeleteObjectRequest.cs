using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed
{
    public class SignedDeleteObjectRequest : SignedBaseRequest
    {
        public SignedDeleteObjectRequest(string url) : base(HttpMethod.DELETE, url) { }
    }
}