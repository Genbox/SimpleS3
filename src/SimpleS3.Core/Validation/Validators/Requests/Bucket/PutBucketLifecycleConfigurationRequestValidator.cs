using FluentValidation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Bucket
{
    public class PutBucketLifecycleConfigurationRequestValidator : BaseRequestValidator<PutBucketLifecycleConfigurationRequest>
    {
        public PutBucketLifecycleConfigurationRequestValidator(IOptions<S3Config> config) : base(config)
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
}