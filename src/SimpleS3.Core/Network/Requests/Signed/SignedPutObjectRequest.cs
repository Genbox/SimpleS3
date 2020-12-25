using System.IO;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Signed
{
    public class SignedPutObjectRequest : SignedBaseRequest, IHasContent
    {
        internal SignedPutObjectRequest() : base(HttpMethod.PUT)
        {
        }

        public SignedPutObjectRequest(string url, Stream? content) : this()
        {
            Initialize(url, content);
        }

        internal void Initialize(string url, Stream? content)
        {
            Content = content;
            Url = url;
        }

        public Stream? Content { get; internal set; }
    }
}