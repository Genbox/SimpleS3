using FluentValidation;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Validation
{
    [UsedImplicitly]
    public abstract class ValidatorBase<T> : AbstractValidator<T>
    {
    }
}