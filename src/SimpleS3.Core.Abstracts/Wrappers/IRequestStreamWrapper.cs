using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers;

/// <summary>Interface for stateless wrappers. USe this when you want to wrap streams on a low level before sending to the
/// S3 API.</summary>
public interface IRequestStreamWrapper
{
    bool IsSupported(IRequest request);

    Stream Wrap(Stream input, IRequest request);
}