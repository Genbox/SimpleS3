using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers;

/// <summary>Interface for stateless wrappers. USe this when you want to wrap streams on a low level before sending to the S3 API.</summary>
public interface IRequestStreamWrapper
{
    /// <summary>Provides the <c>IsSupported</c> API.</summary>
    bool IsSupported(IRequest request);

    /// <summary>Provides the <c>Wrap</c> API.</summary>
    Stream Wrap(Stream input, IRequest request);
}