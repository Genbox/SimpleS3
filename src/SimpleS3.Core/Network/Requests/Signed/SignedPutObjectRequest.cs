using System.IO;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed
{
    public class SignedPutObjectRequest : SignedBaseRequest, IHasContent
    {
        public SignedPutObjectRequest(string url, Stream? content) : base(HttpMethod.PUT, url)
        {
            Content = content;
        }

        public Stream? Content { get; }
    }
}