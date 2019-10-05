using System.IO;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Requests.Objects.Properties
{
    [PublicAPI]
    public interface IHasContent
    {
        Stream Content { get; }
    }
}