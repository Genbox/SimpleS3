using System.Linq;
using FluentValidation.Results;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators;

internal abstract class ConfigValidatorBase<T> : ValidatorBase<T>, IValidateOptions<T> where T : class
{
    public ValidateOptionsResult Validate(string name, T options)
    {
        ValidationResult? results = ValidateAsync(options).Result;

        if (results.IsValid)
            return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(results.Errors.Select(x => x.ErrorMessage));
    }
}