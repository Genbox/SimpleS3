using System.Collections.Generic;
using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts.Response
{
    public interface IResponseMarshal { }

    public interface IResponseMarshal<in TResponse> : IResponseMarshal where TResponse : IResponse
    {
        void MarshalResponse(Config config, TResponse response, IDictionary<string, string> headers, Stream responseStream);
    }
}