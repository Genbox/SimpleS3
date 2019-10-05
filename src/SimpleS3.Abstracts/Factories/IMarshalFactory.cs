using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Abstracts.Factories
{
    public interface IMarshalFactory
    {
        Stream MarshalRequest<T>(T request) where T : IRequest;
        void MarshalResponse<TRequest, TResponse>(TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TRequest : IRequest where TResponse : IResponse;
    }
}