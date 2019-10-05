using FluentValidation;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Requests.Objects.Properties;

namespace Genbox.SimpleS3.Core.Validation.Validators.Requests.Objects.Properties
{
    public class SseCustomerKeyPropertiesValidator : ValidatorBase<ISseCustomerKeyProperties>
    {
        public IConditionBuilder SseCustomerKeyProperties()
        {
            return When(key => key.SseCustomerAlgorithm != SseCustomerAlgorithm.Unknown, () =>
            {
                RuleFor(x => x.SseCustomerAlgorithm).IsInEnum();
                RuleFor(x => x.SseCustomerKey).NotNull().Must(x => x.Length == 32);
                RuleFor(x => x.SseCustomerKeyMd5).NotNull().Must(x => x.Length == 16);
            });
        }
    }
}