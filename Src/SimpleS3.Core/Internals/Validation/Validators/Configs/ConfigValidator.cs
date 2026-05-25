using FluentValidation;
using FluentValidation.Results;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators.Configs;

internal sealed class ConfigValidator : ConfigValidatorBase<SimpleS3Config>
{
    private readonly IServiceProvider _provider;

    public ConfigValidator(IServiceProvider provider)
    {
        _provider = provider;

        RuleFor(x => x.Endpoint).NotEmpty().WithMessage("You must provide an endpoint.");
        RuleFor(x => x.RegionCode).NotEmpty().WithMessage("You must provide a region");
        RuleFor(x => x.PayloadSignatureMode).IsInEnum().Must(x => x != SignatureMode.Unknown).WithMessage("You must provide a valid payload signature mode");
        RuleFor(x => x.NamingMode).IsInEnum().Must(x => x != NamingMode.Unknown).WithMessage("You must provide a valid naming mode");

        // We have to check x.EndPoint != null due to the way validation is run
        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        RuleFor(x => x.NamingMode).IsInEnum().Must(x => x == NamingMode.PathStyle).When(x => x.Endpoint?.Contains('{') == false).WithMessage("You can only use NamingMode.VirtualHost when you specify an endpoint template.");

        RuleFor(x => x.ObjectKeyValidationMode).IsInEnum().Must(x => x != ObjectKeyValidationMode.Unknown).WithMessage("You must provide a valid object key validation mode");

        //See https://docs.aws.amazon.com/AmazonS3/latest/API/sigv4-streaming.html
        RuleFor(x => x.StreamingChunkSize).GreaterThanOrEqualTo(8096).WithMessage("Please specify a chunk size of more than or equal to 8096 bytes");
    }

    public override ValidateOptionsResult Validate(string? name, SimpleS3Config options)
    {
        ValidationResult results = base.Validate(options);
        List<string> failures = results.Errors.Select(x => x.ErrorMessage).ToList();

        if (options.Credentials != null)
        {
            IInputValidator inputValidator = GetInputValidator(name);
            IAccessKeyProtector? protector = _provider.GetService<IAccessKeyProtector>();
            ValidationResult accessKeyResults = new AccessKeyValidator(inputValidator, protector).Validate(options.Credentials);
            failures.AddRange(accessKeyResults.Errors.Select(x => x.ErrorMessage));
        }

        return failures.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }

    private IInputValidator GetInputValidator(string? name)
    {
        string serviceName = string.IsNullOrEmpty(name) ? ServiceBuilderBase.DefaultName : name;

        if (serviceName == ServiceBuilderBase.DefaultName)
        {
            return _provider.GetService<IInputValidator>() ??
                   _provider.GetKeyedService<IInputValidator>(serviceName) ??
                   new NullInputValidator();
        }

        return _provider.GetKeyedService<IInputValidator>(serviceName) ??
               new NullInputValidator();
    }
}