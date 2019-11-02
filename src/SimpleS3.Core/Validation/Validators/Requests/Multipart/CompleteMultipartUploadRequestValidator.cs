using FluentValidation;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Multipart
{
    public class CompleteMultipartUploadRequestValidator : RequestWithObjectKeyBase<CompleteMultipartUploadRequest>
    {
        public CompleteMultipartUploadRequestValidator(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.UploadId).NotEmpty();
        }
    }
}