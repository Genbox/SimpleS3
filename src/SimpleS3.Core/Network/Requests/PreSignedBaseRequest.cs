using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests
{
    public abstract class PreSignedBaseRequest : BaseRequest
    {
        protected PreSignedBaseRequest(HttpMethod method, string url) : base(method)
        {
            Url = url;
        }

        public string Url { get; }
    }
}