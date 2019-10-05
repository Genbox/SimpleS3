namespace Genbox.SimpleS3.Abstracts.Factories
{
    public interface IValidatorFactory
    {
        void ValidateAndThrow<T>(T obj);
    }
}