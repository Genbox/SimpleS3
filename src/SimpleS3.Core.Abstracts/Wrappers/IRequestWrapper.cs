using System.IO;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers
{
    /// <summary>Interface for defining stateful wrappers. This is used for when you wish to wrap the whole request stream.</summary>
    public interface IRequestWrapper
    {
        bool IsSupported(IRequest request);

        Stream Wrap(Stream input, IRequest request);
    }
}