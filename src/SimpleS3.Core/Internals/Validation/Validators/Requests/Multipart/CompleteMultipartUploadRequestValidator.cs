using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Multipart;

internal class CompleteMultipartUploadRequestValidator : RequestValidatorBase<CompleteMultipartUploadRequest>
{
    public CompleteMultipartUploadRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : base(validator, config)
    {
        RuleFor(x => x.UploadParts).Must(x =>
        {
            if (x.Count == 1)
                return true;

            for (int i = 1; i < x.Count; i++)
            {
                if (x[i - 1].PartNumber != x[i].PartNumber - 1)
                    return false;
            }

            return true;
        }).WithMessage("S3 require part numbers to be in ascending order");
    }
}