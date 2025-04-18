﻿using FluentValidation;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Configs;

internal sealed class ConfigValidator : ConfigValidatorBase<SimpleS3Config>
{
    public ConfigValidator(IValidator<IAccessKey?> validator)
    {
        RuleFor(x => x.Endpoint).NotEmpty().WithMessage("You must provide an endpoint.");
        RuleFor(x => x.RegionCode).NotEmpty().WithMessage("You must provide a region");
        RuleFor(x => x.PayloadSignatureMode).IsInEnum().Must(x => x != SignatureMode.Unknown).WithMessage("You must provide a valid payload signature mode");
        RuleFor(x => x.NamingMode).IsInEnum().Must(x => x != NamingMode.Unknown).WithMessage("You must provide a valid naming mode");

        // We have to check x.EndPoint != null due to the way validation is run
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        RuleFor(x => x.NamingMode).IsInEnum().Must(x => x == NamingMode.PathStyle).When(x => x.Endpoint?.Contains('{') == false).WithMessage("You can only use NamingMode.VirtualHost when you specify an endpoint template.");

        RuleFor(x => x.ObjectKeyValidationMode).IsInEnum().Must(x => x != ObjectKeyValidationMode.Unknown).WithMessage("You must provide a valid object key validation mode");
        RuleFor(x => x.Credentials).SetValidator(validator);

        //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html
        RuleFor(x => x.StreamingChunkSize).GreaterThanOrEqualTo(8096).WithMessage("Please specify a chunk size of more than or equal to 8096 bytes");
    }
}