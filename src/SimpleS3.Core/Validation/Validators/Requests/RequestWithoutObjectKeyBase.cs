using FluentValidation;
using Genbox.SimpleS3.Core.Network.Requests;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests
{
    public abstract class RequestWithoutObjectKeyBase<T> : BaseRequestValidator<T> where T : BaseRequest
    {
        protected RequestWithoutObjectKeyBase(IOptions<S3Config> config) : base(config)
        {
            RuleFor(x => x.ObjectKey).Null();
        }
    }
}