using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IResponseMarshal { }

    public interface IResponseMarshal<in TResponse> : IResponseMarshal where TResponse : IResponse
    {
        void MarshalResponse(IConfig config, TResponse response, IDictionary<string, string> headers, Stream responseStream);
    }
}