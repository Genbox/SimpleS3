using FluentValidation;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Requests;
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
                .Must(x => InputValidator.TryValidateObjectKey(x, Level.Level1, out _))
                .When(x => config.Value.ObjectKeyValidationLevel == Level.Level1)
                .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\() are allowed when S3Config.{nameof(S3Config.ObjectKeyValidationLevel)} is set to {nameof(Level.Level1)}")
                .Must(x => InputValidator.TryValidateObjectKey(x, Level.Level2, out _))
                .When(x => config.Value.ObjectKeyValidationLevel == Level.Level2)
                .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\()&$@=;:+ ,? and ASCII codes 0-31 and 127 are allowed when S3Config.{nameof(S3Config.ObjectKeyValidationLevel)} is set to {nameof(Level.Level2)}")
                .Must(x => InputValidator.TryValidateObjectKey(x, Level.Level3, out _))
                .When(x => config.Value.ObjectKeyValidationLevel == Level.Level3)
                .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\()&$@=;:+ ,?\\{{}}^%`[]\"<>~#| and ASCII codes 0-31 and 128-255 are allowed when S3Config.{nameof(S3Config.ObjectKeyValidationLevel)} is set to {nameof(Level.Level3)}");
        }
    }
}