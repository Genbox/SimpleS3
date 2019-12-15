using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Validation.Validators.Configs
{
    public class SecretAccessKeyValidator : AbstractValidator<IAccessKey>
    {
        public SecretAccessKeyValidator()
        {
            RuleFor(x => x.KeyId)
                .NotEmpty().WithMessage("You must provide a key id.")
                .Length(20).WithMessage("The key id must be 20 characters long.");

            RuleFor(x => x.AccessKey)
                .NotNull().WithMessage("You must provide a secret access key.");
        }
    }
}