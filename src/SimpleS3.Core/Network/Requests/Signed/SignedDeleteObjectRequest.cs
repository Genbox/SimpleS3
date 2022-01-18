using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed;

public class SignedDeleteObjectRequest : SignedBaseRequest
{
    internal SignedDeleteObjectRequest() : base(HttpMethodType.DELETE) { }

    public SignedDeleteObjectRequest(string url) : this()
    {
        Initialize(url);
    }

    internal void Initialize(string url)
    {
        Url = url;
    }
}