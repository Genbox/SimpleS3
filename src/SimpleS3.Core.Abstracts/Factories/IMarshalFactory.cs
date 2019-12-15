using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts.Factories
{
    public interface IMarshalFactory
    {
        Stream MarshalRequest<T>(T request, IS3Config config) where T : IRequest;
        void MarshalResponse<TRequest, TResponse>(IS3Config config, TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TRequest : IRequest where TResponse : IResponse;
    }
}