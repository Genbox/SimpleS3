using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed;

public class SignedPutObjectRequest : SignedBaseRequest, IHasContent
{
    internal SignedPutObjectRequest() : base(HttpMethodType.PUT) {}

    public SignedPutObjectRequest(string url, Stream? content) : this()
    {
        Initialize(url, content);
    }

    public Stream? Content { get; internal set; }

    internal void Initialize(string url, Stream? content)
    {
        Content = content;
        Url = url;
    }
}