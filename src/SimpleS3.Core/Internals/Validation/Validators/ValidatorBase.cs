using FluentValidation;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
internal abstract class ValidatorBase<T> : AbstractValidator<T> {}