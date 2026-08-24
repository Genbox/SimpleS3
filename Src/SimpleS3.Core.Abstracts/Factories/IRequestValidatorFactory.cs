using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Factories;

/// <summary>Provides the <c>IRequestValidatorFactory</c> API.</summary>
public interface IRequestValidatorFactory
{
    /// <summary>Provides the <c>ValidateAndThrow&lt;T&gt;</c> API.</summary>
    void ValidateAndThrow<T>(T obj) where T : IRequest;
}