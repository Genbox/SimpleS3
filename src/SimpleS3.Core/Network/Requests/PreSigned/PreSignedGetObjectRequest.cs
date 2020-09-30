using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.PreSigned
{
    public class PreSignedGetObjectRequest : PreSignedBaseRequest
    {
        public PreSignedGetObjectRequest(string url) : base(HttpMethod.GET, url) { }
    }
}