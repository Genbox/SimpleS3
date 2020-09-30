namespace Genbox.SimpleS3.Core.Abstracts.Factories
{
    public interface IPostMapperFactory
    {
        void PostMap<TRequest, TResponse>(IConfig config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse;
    }
}