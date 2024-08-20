using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Objects;

internal sealed class ListObjectsRequestValidator : RequestValidatorBase<ListObjectsRequest>
{
    public ListObjectsRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : base(validator, config)
    {
        RuleFor(x => x.MaxKeys).GreaterThan(0).LessThanOrEqualTo(1000).When(x => x.MaxKeys != null);
    }
}