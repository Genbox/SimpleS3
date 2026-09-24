using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Objects;

internal sealed class CopyObjectRequestValidator : RequestValidatorBase<CopyObjectRequest>
{
    public CopyObjectRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : base(validator, config)
    {
        When(x => x.SourceSseCustomerAlgorithm != SseCustomerAlgorithm.Unknown || x.SourceSseCustomerKey != null || x.SourceSseCustomerKeyMd5 != null, () =>
        {
            RuleFor(x => x.SourceSseCustomerAlgorithm).IsInEnum().NotEqual(SseCustomerAlgorithm.Unknown);
            RuleFor(x => x.SourceSseCustomerKey).NotNull().Must(x => x != null && x.Length == 32);
            RuleFor(x => x.SourceSseCustomerKeyMd5).NotNull().Must(x => x != null && x.Length == 16);
        });
    }
}