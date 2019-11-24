using FluentValidation;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class ListObjectsRequestValidator : BaseRequestValidator<ListObjectsRequest>
    {
        public ListObjectsRequestValidator(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.MaxKeys).GreaterThan(0).LessThanOrEqualTo(1000).When(x => x.MaxKeys != null);
        }
    }
}