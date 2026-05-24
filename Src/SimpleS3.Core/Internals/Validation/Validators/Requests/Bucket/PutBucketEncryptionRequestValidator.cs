using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Bucket;

internal sealed class PutBucketEncryptionRequestValidator : RequestValidatorBase<PutBucketEncryptionRequest>
{
    public PutBucketEncryptionRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : base(validator, config)
    {
        RuleFor(x => x.Rules).NotEmpty();
        RuleForEach(x => x.Rules).Must(x => x.SseAlgorithm != SseAlgorithm.Unknown || x.BlockedEncryptionTypes.Count > 0 || x.BucketKeyEnabled.HasValue);
    }
}
