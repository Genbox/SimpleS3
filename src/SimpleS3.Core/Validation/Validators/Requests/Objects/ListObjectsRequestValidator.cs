using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects
{
    public class ListObjectsRequestValidator : BaseRequestValidator<ListObjectsRequest>
    {
        public ListObjectsRequestValidator(IInputValidator validator, Config config) : base(validator, config)
        {
            RuleFor(x => x.MaxKeys).GreaterThan(0).LessThanOrEqualTo(1000).When(x => x.MaxKeys != null);
        }
    }
}