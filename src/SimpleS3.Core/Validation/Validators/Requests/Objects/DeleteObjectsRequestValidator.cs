using FluentValidation;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class DeleteObjectsRequestValidator : RequestWithObjectKeyBase<DeleteObjectsRequest>
    {
        public DeleteObjectsRequestValidator(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.Objects).NotEmpty();
        }
    }
}