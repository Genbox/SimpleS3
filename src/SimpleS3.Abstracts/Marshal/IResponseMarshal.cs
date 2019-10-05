using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Abstracts.Marshal
{
    public interface IResponseMarshal
    {
    }

    public interface IResponseMarshal<in TRequest, in TResponse> : IResponseMarshal where TRequest : IRequest where TResponse : IResponse
    {
        void MarshalResponse(TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream);
    }
}