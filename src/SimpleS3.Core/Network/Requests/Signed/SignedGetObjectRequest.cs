using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed;

public class SignedGetObjectRequest : SignedBaseRequest
{
    internal SignedGetObjectRequest() : base(HttpMethodType.GET) { }

    public SignedGetObjectRequest(string url) : this()
    {
        Initialize(url);
    }

    internal void Initialize(string url)
    {
        Url = url;
    }
}