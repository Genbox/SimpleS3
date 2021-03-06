﻿using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Requests.Multipart
{
    internal class ListMultipartUploadsRequestValidator : BaseRequestValidator<ListMultipartUploadsRequest>
    {
        public ListMultipartUploadsRequestValidator(IInputValidator validator, IOptions<Config> config) : base(validator, config)
        {
            RuleFor(x => x.MaxUploads).GreaterThan(0).LessThanOrEqualTo(1000).When(x => x.MaxUploads != null);
        }
    }
}