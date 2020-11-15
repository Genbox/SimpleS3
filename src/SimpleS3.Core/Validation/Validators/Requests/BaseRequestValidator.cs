using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests
{
    public abstract class BaseRequestValidator<T> : ValidatorBase<T> where T : BaseRequest
    {
        protected BaseRequestValidator(IInputValidator validator, Config config)
        {
            RuleFor(x => x.Method).IsInEnum().Must(x => x != HttpMethod.Unknown);

            When(x => x is IHasUploadId, () => RuleFor(x => ((IHasUploadId)x).UploadId).NotEmpty());

            When(x => x is IHasSseCustomerKey key && key.SseCustomerAlgorithm != SseCustomerAlgorithm.Unknown, () =>
            {
                RuleFor(x => ((IHasSseCustomerKey)x).SseCustomerAlgorithm).NotEmpty();
                RuleFor(x => ((IHasSseCustomerKey)x).SseCustomerKey).NotNull().Must(x => x!.Length == 32);
                RuleFor(x => ((IHasSseCustomerKey)x).SseCustomerKeyMd5).NotNull().Must(x => x!.Length == 16);
            });

            //See https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html
            //- Can contain multiple DNS labels, which must start with lowercase letter or digit
            //- Other than the start character, it must also contain hyphens
            //- Must be between 3 and 63 long
            When(x => x is IHasBucketName,
                () => RuleFor(x => ((IHasBucketName)x).BucketName)
                      .Length(3, 64)
                      .Must(x => validator.TryValidateBucketName(x, out _))
                      .When(x => config.EnableBucketNameValidation)
                      .WithMessage("Amazon recommends naming buckets to be valid DNS names, as you can't change the name later on. To turn off DNS name validation, set S3Config.EnableBucketNameValidation to false"));

            When(x => x is IHasObjectKey,
                () => RuleFor(x => ((IHasObjectKey)x).ObjectKey)
                      .NotEmpty()
                      .WithMessage("You must provide an object key.")
                      .Length(1, 1024)
                      .WithMessage("The object key length must be between 1 and 1024 characters")
                      .Must(x => validator.TryValidateObjectKey(x, ObjectKeyValidationMode.SafeMode, out _))
                      .When(x => config.ObjectKeyValidationMode == ObjectKeyValidationMode.SafeMode)
                      .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\() are allowed when S3Config.{nameof(Config.ObjectKeyValidationMode)} is set to {nameof(ObjectKeyValidationMode.SafeMode)}")
                      .Must(x => validator.TryValidateObjectKey(x, ObjectKeyValidationMode.AsciiMode, out _))
                      .When(x => config.ObjectKeyValidationMode == ObjectKeyValidationMode.AsciiMode)
                      .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\()&$@=;:+ ,? and ASCII codes 0-31 and 127 are allowed when S3Config.{nameof(Config.ObjectKeyValidationMode)} is set to {nameof(ObjectKeyValidationMode.AsciiMode)}")
                      .Must(x => validator.TryValidateObjectKey(x, ObjectKeyValidationMode.ExtendedAsciiMode, out _))
                      .When(x => config.ObjectKeyValidationMode == ObjectKeyValidationMode.ExtendedAsciiMode)
                      .WithMessage($"Only a-z, A-Z, 0-9 and the characters /!-_.*\\()&$@=;:+ ,?\\{{}}^%`[]\"<>~#| and ASCII codes 0-31 and 128-255 are allowed when S3Config.{nameof(Config.ObjectKeyValidationMode)} is set to {nameof(ObjectKeyValidationMode.ExtendedAsciiMode)}"));
        }
    }
}