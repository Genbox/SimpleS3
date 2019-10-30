using FluentValidation;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests;
using Genbox.SimpleS3.Utils;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests
{
    public abstract class BaseRequestValidator<T> : ValidatorBase<T> where T : BaseRequest
    {
        protected BaseRequestValidator(IOptions<S3Config> config)
        {
            RuleFor(x => x.Method).IsInEnum().Must(x => x != HttpMethod.Unknown);

            //See https://docs.aws.amazon.com/AmazonS3/latest/dev/BucketRestrictions.html
            //- Can contain multiple DNS labels, which must start with lowercase letter or digit
            //- Other than the start character, it must also contain hyphens
            //- Must be between 3 and 63 long
            RuleFor(x => x.BucketName)
                .Length(3, 64)
                .Must(x => InputValidator.TryValidateBucketName(x, out _))
                .When(x => config.Value.EnableBucketNameValidation)
                .WithMessage("Amazon recommends naming buckets to be valid DNS names, as you can't change the name later on. To turn off DNS name validation, set S3Config.EnableBucketNameValidation to false");
        }
    }
}