using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers;

/// <summary>Interface for defining stateful wrappers. This is used for when you wish to wrap the whole request stream.</summary>
public interface IRequestWrapper
{
    /// <summary>Provides the <c>IsSupported</c> API.</summary>
    bool IsSupported(IRequest request);

    /// <summary>Provides the <c>Wrap</c> API.</summary>
    Stream Wrap(Stream input, IRequest request);
}