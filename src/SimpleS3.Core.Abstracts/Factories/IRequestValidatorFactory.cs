using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Factories
{
    public interface IRequestValidatorFactory
    {
        void ValidateAndThrow<T>(T obj) where T : IRequest;
    }
}