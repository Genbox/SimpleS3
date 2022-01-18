using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed;

public class SignedHeadObjectRequest : SignedBaseRequest
{
    internal SignedHeadObjectRequest() : base(HttpMethodType.HEAD) { }

    public SignedHeadObjectRequest(string url) : this()
    {
        Initialize(url);
    }

    internal void Initialize(string url)
    {
        Url = url;
    }
}