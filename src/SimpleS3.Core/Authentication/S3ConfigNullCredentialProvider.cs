using Genbox.SimpleS3.Abstracts.Authentication;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Authentication
{
    public class S3ConfigNullCredentialProvider : CredentialProviderBase, IS3ConfigCredentialProvider
    {
        public S3ConfigNullCredentialProvider(IOptions<S3Config> options) : base(options?.Value.Credentials.AccessKey)
        {
        }
    }
}