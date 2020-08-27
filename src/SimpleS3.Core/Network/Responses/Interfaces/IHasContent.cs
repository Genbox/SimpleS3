using System.IO;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces
{
    [PublicAPI]
    public interface IHasContent
    {
        Stream Content { get; }
    }
}