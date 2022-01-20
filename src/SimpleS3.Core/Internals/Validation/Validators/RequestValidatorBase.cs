using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators;

internal abstract class RequestValidatorBase<T> : ValidatorBase<T> where T : IRequest
{
    private readonly IInputValidator _validator;
    private readonly SimpleS3Config _cfg;

    protected RequestValidatorBase(IInputValidator validator, IOptions<SimpleS3Config> config)
    {
        _validator = validator;
        _cfg = config.Value;

        RuleFor(x => x.Method).IsInEnum().Must(x => x != HttpMethodType.Unknown);

        When(x => x is IHasUploadId, () => RuleFor(x => ((IHasUploadId)x).UploadId).NotEmpty());

        When(x => x is IHasSseCustomerKey key && key.SseCustomerAlgorithm != SseCustomerAlgorithm.Unknown, () =>
        {
            RuleFor(x => ((IHasSseCustomerKey)x).SseCustomerAlgorithm).NotEmpty();
            RuleFor(x => ((IHasSseCustomerKey)x).SseCustomerKey).NotNull().Must(x => x != null && x.Length == 32);
            RuleFor(x => ((IHasSseCustomerKey)x).SseCustomerKeyMd5).NotNull().Must(x => x != null && x.Length == 16);
        });

        //See https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html
        //- Can contain multiple DNS labels, which must start with lowercase letter or digit
        //- Other than the start character, it must also contain hyphens
        //- Must be between 3 and 63 long
        When(x => x is IHasBucketName,
            () => RuleFor(x => ((IHasBucketName)x).BucketName)
                .Custom(ValidateBucketName));

        When(x => x is IHasObjectKey,
            () => RuleFor(x => ((IHasObjectKey)x).ObjectKey)
                .Custom(ValidateObjectKey));
    }

    private void ValidateObjectKey(string input, ValidationContext<T> context)
    {
        if (!_validator.TryValidateObjectKey(input, _cfg.ObjectKeyValidationMode, out ValidationStatus status, out string? allowed))
            context.AddFailure("Invalid object key: " + ValidationMessages.GetMessage(status, allowed));
    }

    private void ValidateBucketName(string input, ValidationContext<T> context)
    {
        if (!_validator.TryValidateBucketName(input, _cfg.BucketNameValidationMode, out ValidationStatus status, out string? allowed))
            context.AddFailure("Invalid bucket name: " + ValidationMessages.GetMessage(status, allowed));
    }
}