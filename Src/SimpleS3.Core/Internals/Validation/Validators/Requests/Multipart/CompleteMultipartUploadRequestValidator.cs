using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Multipart;

internal sealed class CompleteMultipartUploadRequestValidator : RequestValidatorBase<CompleteMultipartUploadRequest>
{
    public CompleteMultipartUploadRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : base(validator, config)
    {
        RuleFor(x => x.UploadParts).Must(x => x is { Count: >= 1 and <= 10_000 })
                                   .WithMessage("S3 allows between 1 and 10,000 parts in a multipart upload");

        RuleForEach(x => x.UploadParts).Must(x => x.PartNumber is >= 1 and <= 10_000)
                                       .WithMessage("S3 part numbers must be between 1 and 10,000");

        RuleFor(x => x.UploadParts).Must(x =>
        {
            for (int i = 1; i < x.Count; i++)
            {
                if (x[i - 1].PartNumber >= x[i].PartNumber)
                    return false;
            }

            return true;
        }).WithMessage("S3 requires part numbers to be in ascending order");
    }
}