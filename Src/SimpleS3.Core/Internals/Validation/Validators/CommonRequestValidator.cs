using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Provider;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Validation.Validators;

internal sealed class CommonRequestValidator(IInputValidator validator, IOptions<SimpleS3Config> config) : RequestValidatorBase<IRequest>(validator, config);