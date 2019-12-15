using Genbox.SimpleS3.Core.Misc;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces
{
    [PublicAPI]
    public interface IHasContent
    {
        ContentReader Content { get; }
    }
}