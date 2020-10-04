using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Multipart
{
    public class ListMultipartUploadsRequestValidator : BaseRequestValidator<ListMultipartUploadsRequest>
    {
        public ListMultipartUploadsRequestValidator(IInputValidator validator, Config config) : base(validator, config)
        {
            RuleFor(x => x.MaxUploads).GreaterThan(0).LessThanOrEqualTo(1000).When(x => x.MaxUploads != null);
        }
    }
}