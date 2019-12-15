using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Validation.Validators.Configs
{
    public class S3ConfigValidator : AbstractValidator<S3Config>
    {
        public S3ConfigValidator(IValidator<IAccessKey> validator)
        {
            RuleFor(x => x.Region).IsInEnum().Must(x => x != AwsRegion.Unknown).WithMessage("You must provide a region.");
            RuleFor(x => x.Credentials).NotNull().WithMessage("You must provide credentials.").SetValidator(validator);
        }
    }
}