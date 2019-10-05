using FluentValidation;
using Genbox.SimpleS3.Core.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Buckets
{
    public class GetBucketRequestValidator : BaseRequestValidator<GetBucketRequest>
    {
        public GetBucketRequestValidator(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.Resource).Empty();
            RuleFor(x => x.MaxKeys).GreaterThan(0).LessThanOrEqualTo(1000).When(x => x.MaxKeys != null);
        }
    }
}