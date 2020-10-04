using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IRequestMarshal { }

    public interface IRequestMarshal<in T> : IRequestMarshal where T : IRequest
    {
        Stream? MarshalRequest(T request, Config config);
    }
}