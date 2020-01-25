using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts.Factories
{
    public interface IMarshalFactory
    {
        Stream MarshalRequest<T>(T request, IConfig config) where T : IRequest;
        void MarshalResponse<TRequest, TResponse>(IConfig config, TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TRequest : IRequest where TResponse : IResponse;
    }
}