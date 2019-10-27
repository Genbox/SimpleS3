using FluentValidation;
using Genbox.SimpleS3.Core.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class HeadObjectRequestValidator : BaseRequestValidator<HeadObjectRequest>
    {
        public HeadObjectRequestValidator(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.ObjectKey).NotEmpty().WithMessage("You must provide an object key.");
        }
    }
}