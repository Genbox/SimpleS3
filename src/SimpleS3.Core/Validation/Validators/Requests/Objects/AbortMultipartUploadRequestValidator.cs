using Genbox.SimpleS3.Core.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class AbortMultipartUploadRequestValidator : RequestWithObjectKeyBase<AbortMultipartUploadRequest>
    {
        public AbortMultipartUploadRequestValidator(IOptions<S3Config> config) : base(config)
        {
        }
    }
}