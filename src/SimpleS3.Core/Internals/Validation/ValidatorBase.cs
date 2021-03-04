using FluentValidation;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Validation
{
    [UsedImplicitly]
    internal abstract class ValidatorBase<T> : AbstractValidator<T> { }
}