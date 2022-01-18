using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Bucket;

internal class PutBucketLifecycleConfigurationRequestValidator : RequestValidatorBase<PutBucketLifecycleConfigurationRequest>
{
    public PutBucketLifecycleConfigurationRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : base(validator, config)
    {
        RuleFor(x => x.Rules).NotEmpty();
        RuleForEach(x => x.Rules)
            .ChildRules(x2 =>
            {
                x2.RuleForEach(x3 => x3.Transitions)
                    .ChildRules(x4 =>
                    {
                        x4.RuleFor(x5 => x5.TransitionAfterDays)
                            .GreaterThanOrEqualTo(30)
                            .When(x5 => x5.StorageClass == StorageClass.OneZoneIa);
                    });
            });
    }
}