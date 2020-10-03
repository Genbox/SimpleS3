using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed
{
    public class SignedGetObjectRequest : SignedBaseRequest
    {
        public SignedGetObjectRequest(string url) : base(HttpMethod.GET, url) { }
    }
}