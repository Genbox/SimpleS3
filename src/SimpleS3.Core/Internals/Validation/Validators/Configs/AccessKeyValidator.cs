using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts.Authentication;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Configs
{
    internal class AccessKeyValidator : ValidatorBase<IAccessKey>
    {
        public AccessKeyValidator()
        {
            RuleFor(x => x.KeyId)
                .NotEmpty().WithMessage("You must provide a key id.")
                .Length(20).WithMessage("The key id must be 20 characters long.");

            RuleFor(x => x.SecretKey)
                .NotNull().WithMessage("You must provide a secret access key.");
        }
    }
}