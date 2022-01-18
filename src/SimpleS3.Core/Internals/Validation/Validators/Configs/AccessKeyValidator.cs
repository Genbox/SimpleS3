using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Configs;

internal class AccessKeyValidator : ValidatorBase<IAccessKey>
{
    private readonly IInputValidator _inputValidator;

    public AccessKeyValidator(IInputValidator inputValidator)
    {
        _inputValidator = inputValidator;

        RuleFor(x => x.KeyId)
            .NotEmpty().WithMessage("You must provide a key id")
            .Custom(ValidateKeyId);

        RuleFor(x => x.SecretKey)
            .NotNull().WithMessage("You must provide a secret key")
            .Custom(ValidateSecretKey);
    }

    private void ValidateKeyId(string input, ValidationContext<IAccessKey> context)
    {
        if (!_inputValidator.TryValidateKeyId(input, out ValidationStatus status, out string? allowed))
            context.AddFailure("Invalid key id: " + ValidationMessages.GetMessage(status, allowed));
    }

    private void ValidateSecretKey(byte[] input, ValidationContext<IAccessKey> context)
    {
        if (!_inputValidator.TryValidateAccessKey(input, out ValidationStatus status, out string? allowed))
            context.AddFailure("Invalid secret key: " + ValidationMessages.GetMessage(status, allowed));
    }
}