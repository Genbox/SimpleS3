using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.PreSigned
{
    public class PreSignedDeleteObjectRequest : PreSignedBaseRequest
    {
        public PreSignedDeleteObjectRequest(string url) : base(HttpMethod.DELETE, url) { }
    }
}