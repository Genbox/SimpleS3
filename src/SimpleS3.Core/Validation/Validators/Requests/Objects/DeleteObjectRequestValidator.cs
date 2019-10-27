using Genbox.SimpleS3.Core.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class DeleteObjectRequestValidator : RequestWithObjectKeyBase<DeleteObjectRequest>
    {
        public DeleteObjectRequestValidator(IOptions<S3Config> config) : base(config)
        {
        }
    }
}