using Genbox.SimpleS3.Core.Abstracts.Response;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

[PublicAPI]
public interface IHasContent : IDisposable
{
    ContentStream Content { get; }
}