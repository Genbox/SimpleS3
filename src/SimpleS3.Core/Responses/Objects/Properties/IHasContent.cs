using Genbox.SimpleS3.Core.Misc;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Objects.Properties
{
    [PublicAPI]
    public interface IHasContent
    {
        ContentReader Content { get; }
    }
}