using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class DeleteObjectsRequestValidator : BaseRequestValidator<DeleteObjectsRequest>
    {
        public DeleteObjectsRequestValidator(IInputValidator validator, Config config) : base(validator, config)
        {
            RuleFor(x => x.Objects).NotEmpty();
        }
    }
}