using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Validation.Validators.Configs
{
    [UsedImplicitly]
    public class S3ConfigValidator : AbstractValidator<S3Config>
    {
        public S3ConfigValidator(IValidator<IAccessKey> validator = null)
        {
            RuleFor(x => x.Region).IsInEnum().Must(x => x != AwsRegion.Unknown).WithMessage("You must provide a region.");
            RuleFor(x => x.PayloadSignatureMode).IsInEnum().Must(x => x != SignatureMode.Unknown).WithMessage("You must provide a valid payload signature mode.");
            RuleFor(x => x.NamingMode).IsInEnum().Must(x => x != NamingMode.Unknown).WithMessage("You must provide a valid naming mode.");
            RuleFor(x => x.ObjectKeyValidationMode).IsInEnum().Must(x => x != KeyValidationMode.Unknown).WithMessage("You must provide a valid object key validation mode.");

            IRuleBuilderOptions<S3Config, IAccessKey> validatorRule = RuleFor(x => x.Credentials).NotNull().WithMessage("You must provide credentials.");

            if (validator != null)
                validatorRule.SetValidator(validator);

            //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html
            RuleFor(x => x.StreamingChunkSize).GreaterThanOrEqualTo(8096).WithMessage("Please specify a chunk size of more than or equal to 8096 bytes.");
        }
    }
}