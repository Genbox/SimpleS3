using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Factories
{
    public interface IMarshalFactory
    {
        Stream? MarshalRequest<TRequest>(SimpleS3Config config, TRequest request) where TRequest : IRequest;
        void MarshalResponse<TResponse>(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TResponse : IResponse;
    }
}