using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts.Factories
{
    public interface IMarshalFactory
    {
        Stream? MarshalRequest<TRequest>(TRequest request, IConfig config) where TRequest : IRequest;
        void MarshalResponse<TResponse>(IConfig config, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TResponse : IResponse;
    }
}