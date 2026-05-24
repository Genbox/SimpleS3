using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Configs;

internal sealed class AccessKeyValidator : ValidatorBase<IAccessKey>
{
    private readonly IInputValidator _inputValidator;
    private readonly IAccessKeyProtector? _protector;

    public AccessKeyValidator(IInputValidator inputValidator, IAccessKeyProtector? protector = null)
    {
        _inputValidator = inputValidator;
        _protector = protector;

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
        if (_inputValidator.TryValidateAccessKey(input, out _, out _))
            return;

        byte[] accessKey;

        try
        {
            accessKey = KeyHelper.UnprotectKey(input, _protector);
        }
        catch
        {
            AddValidationFailure(input, context);
            return;
        }

        try
        {
            AddValidationFailure(accessKey, context);
        }
        finally
        {
            if (!ReferenceEquals(accessKey, input))
                Array.Clear(accessKey, 0, accessKey.Length);
        }
    }

    private void AddValidationFailure(byte[] input, ValidationContext<IAccessKey> context)
    {
        if (!_inputValidator.TryValidateAccessKey(input, out ValidationStatus status, out string? allowed))
            context.AddFailure("Invalid secret key: " + ValidationMessages.GetMessage(status, allowed));
    }
}