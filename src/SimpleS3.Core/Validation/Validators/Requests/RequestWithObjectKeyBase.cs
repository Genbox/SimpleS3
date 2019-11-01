using FluentValidation;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests;
using Genbox.SimpleS3.Utils;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests
{
    public abstract class RequestWithObjectKeyBase<T> : BaseRequestValidator<T> where T : BaseRequest
    {
        protected RequestWithObjectKeyBase(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.ObjectKey)
                .NotEmpty()
                .WithMessage("You must provide an object key.")
                .Length(1, 1024)
                .WithMessage("The object key length must be between 1 and 1024 characters")
                .Must(x => InputValidator.TryValidateObjectKey(x, KeyValidationMode.SafeMode, out _))
                .When(x => config.Value.ObjectKeyValidationMode == KeyValidationMode.SafeMode)
                .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\() are allowed when S3Config.{nameof(S3Config.ObjectKeyValidationMode)} is set to {nameof(KeyValidationMode.SafeMode)}")
                .Must(x => InputValidator.TryValidateObjectKey(x, KeyValidationMode.AsciiMode, out _))
                .When(x => config.Value.ObjectKeyValidationMode == KeyValidationMode.AsciiMode)
                .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\()&$@=;:+ ,? and ASCII codes 0-31 and 127 are allowed when S3Config.{nameof(S3Config.ObjectKeyValidationMode)} is set to {nameof(KeyValidationMode.AsciiMode)}")
                .Must(x => InputValidator.TryValidateObjectKey(x, KeyValidationMode.ExtendedAsciiMode, out _))
                .When(x => config.Value.ObjectKeyValidationMode == KeyValidationMode.ExtendedAsciiMode)
                .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\()&$@=;:+ ,?\\{{}}^%`[]\"<>~#| and ASCII codes 0-31 and 128-255 are allowed when S3Config.{nameof(S3Config.ObjectKeyValidationMode)} is set to {nameof(KeyValidationMode.ExtendedAsciiMode)}");
        }
    }
}