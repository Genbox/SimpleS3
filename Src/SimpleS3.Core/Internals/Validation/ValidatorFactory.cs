using FluentValidation;
using FluentValidation.Results;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Validation.Validators;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation;

[PublicAPI]
internal sealed class ValidatorFactory : IRequestValidatorFactory
{
    private readonly Dictionary<Type, IValidator> _validators;

    public ValidatorFactory(IEnumerable<IValidator> validators)
    {
        //The validators that come in here all inherit from AbstractValidator<T>
        //So we take out the generic parameter type and use that for our internal lookup

        _validators = validators.Where(x => IsRequestValidator(x.GetType())).ToDictionary(x => GetRequestType(x.GetType()), x => x);
    }

    internal static ValidatorFactory Create(IServiceProvider provider, IOptions<SimpleS3Config> options, IInputValidator inputValidator)
    {
        Type[] validatorTypes = TypeHelper.GetInstanceTypesInheritedFrom(typeof(IValidator), typeof(ValidatorFactory).Assembly)
            .Where(IsRequestValidator)
            .ToArray();

        IValidator[] validators = new IValidator[validatorTypes.Length];

        for (int i = 0; i < validatorTypes.Length; i++)
            validators[i] = (IValidator)ActivatorUtilities.CreateInstance(provider, validatorTypes[i], inputValidator, options);

        return new ValidatorFactory(validators);
    }

    private static bool IsRequestValidator(Type type)
    {
        return GetRequestValidatorBase(type) != null;
    }

    private static Type GetRequestType(Type type)
    {
        Type? requestValidatorBase = GetRequestValidatorBase(type);
        Validator.RequireNotNull(requestValidatorBase);
        return requestValidatorBase.GetGenericArguments()[0];
    }

    private static Type? GetRequestValidatorBase(Type type)
    {
        for (Type? baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(RequestValidatorBase<>))
                return baseType;
        }

        return null;
    }

    public void ValidateAndThrow<T>(T obj) where T : IRequest
    {
        if (_validators.TryGetValue(typeof(T), out IValidator? validator))
        {
            ValidationResult result = ((IValidator<T>)validator).Validate(obj);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }
    }
}