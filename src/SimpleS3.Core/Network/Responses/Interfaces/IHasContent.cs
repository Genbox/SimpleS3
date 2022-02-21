using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

[PublicAPI]
public interface IHasContent : IDisposable
{
    Stream Content { get; }
}