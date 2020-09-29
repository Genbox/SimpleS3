using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts.Marshallers
{
    public interface IResponseMarshal { }

    public interface IResponseMarshal<in TRequest, in TResponse> : IResponseMarshal where TRequest : IRequest where TResponse : IResponse
    {
        void MarshalResponse(IConfig config, TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream);
    }
}