using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Wrappers;

/// <summary>Provides the <c>IResponseStreamWrapper</c> API.</summary>
public interface IResponseStreamWrapper
{
    /// <summary>Provides the <c>IsSupported</c> API.</summary>
    bool IsSupported(IResponse response);

    /// <summary>Provides the <c>Wrap</c> API.</summary>
    Stream Wrap(Stream input, IResponse response);
}