using Genbox.SimpleS3.Cli.Core.Structs;
using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Cli.Core.Abstracts;

public interface IFileProvider
{
    IAsyncEnumerable<OutPathData> GetFiles(ISimpleClient client, PathData src, PathData dst);
}