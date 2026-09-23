using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Bucket;

internal sealed class ListBucketsRequestValidator : RequestValidatorBase<ListBucketsRequest>
{
    public ListBucketsRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : base(validator, config)
    {
        RuleFor(x => x.MaxBuckets).InclusiveBetween(1, 10000).When(x => x.MaxBuckets != null);
    }
}