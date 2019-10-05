using System.IO;

namespace Genbox.SimpleS3.Abstracts.Wrappers
{
    public interface IResponseStreamWrapper
    {
        bool IsSupported(IResponse response);

        Stream Wrap(Stream input, IResponse response);
    }
}