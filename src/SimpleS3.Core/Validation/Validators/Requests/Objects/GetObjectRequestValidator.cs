using FluentValidation;
using Genbox.SimpleS3.Core.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class GetObjectRequestValidator : BaseRequestValidator<GetObjectRequest>
    {
        public GetObjectRequestValidator(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.Resource).NotEmpty().WithMessage("You must provide a resource.");
        }
    }
}