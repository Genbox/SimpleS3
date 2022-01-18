using FluentValidation;
using FluentValidation.Results;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Validation;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Validation;

[PublicAPI]
internal class ValidatorFactory : IRequestValidatorFactory
{
    private readonly IDictionary<Type, IValidator> _validators;

    public ValidatorFactory(IEnumerable<IValidator> validators)
    {
        //The validators that come in here all inherit from AbstractValidator<T>
        //So we take out the generic parameter type and use that for our internal lookup

        _validators = validators.ToDictionary(x =>
        {
            Type type = x.GetType();
            Type? baseType = type.BaseType;

            Validator.RequireNotNull(baseType, nameof(baseType));

            Type[] args = baseType.GetGenericArguments();

            return args[0];
        }, x => x);
    }

    public void ValidateAndThrow<T>(T obj) where T : IRequest
    {
        if (_validators.TryGetValue(typeof(T), out IValidator validator))
        {
            ValidationResult result = ((IValidator<T>)validator).Validate(obj);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }
    }
}