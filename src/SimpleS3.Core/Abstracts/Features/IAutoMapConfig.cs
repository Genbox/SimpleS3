using System;

namespace Genbox.SimpleS3.Core.Abstracts.Features;

public interface IAutoMapConfig
{
    Func<Type, bool> AutoMapDisabledFor { get; }
}