using FluentValidation;
using Genbox.SimpleS3.Core.Requests;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests
{
    public abstract class RequestWithObjectKeyBase<T> : BaseRequestValidator<T> where T : BaseRequest
    {
        protected RequestWithObjectKeyBase(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.ObjectKey).NotEmpty().WithMessage("You must provide an object key.");
        }
    }
}