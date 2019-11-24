using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Multipart
{
    public class UploadPartRequestValidator : BaseRequestValidator<UploadPartRequest>
    {
        public UploadPartRequestValidator(IOptions<S3Config> config) : base(config)
        {
        }
    }
}