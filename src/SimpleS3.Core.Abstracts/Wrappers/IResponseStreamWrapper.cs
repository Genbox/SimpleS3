using System.IO;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers
{
    public interface IResponseStreamWrapper
    {
        bool IsSupported(IResponse response);

        Stream Wrap(Stream input, IResponse response);
    }
}