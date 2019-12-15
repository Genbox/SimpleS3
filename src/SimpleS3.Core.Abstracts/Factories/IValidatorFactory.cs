namespace Genbox.SimpleS3.Core.Abstracts.Factories
{
    public interface IValidatorFactory
    {
        void ValidateAndThrow<T>(T obj);
    }
}